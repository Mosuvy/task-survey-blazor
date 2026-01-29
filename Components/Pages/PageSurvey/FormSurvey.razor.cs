using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Text.Json;
using TaskSurvey.Infrastructure.Data;
using TaskSurvey.Infrastructure.DTOs.SurveyDTOs;
using TaskSurvey.Infrastructure.DTOs.TemplateDTOs;
using TaskSurvey.Infrastructure.DTOs.UserDTOs;
using TaskSurvey.Infrastructure.Models;
using TaskSurvey.Infrastructure.Services.IServices;
using TaskSurvey.Infrastructure.Utils;
using TaskSurvey.StateServices;

namespace TaskSurvey.Components.Pages.FormSurvey;

public partial class FormSurvey : ComponentBase
{
    [Inject] private ISurveyService SurveyService { get; set; } = default!;
    [Inject] private ITemplateService TemplateService { get; set; } = default!;
    [Inject] private IUserService UserService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private AuthState AuthState { get; set; } = default!;
    [Inject] private IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private SurveyHeaderRequestDTO reqDto = new() { SurveyItems = new() };
    private List<TemplateHeaderResponseDTO> allTemplates = new();
    private UserResponseDTO? requesterData;
    
    private string? currentSurveyId;
    private string selectedTemplateName = "";
    private string selectedTemplateTheme = "";
    private string searchQuery = "";
    private string? alertMessage;
    private string? successMessage;
    private int userPositionId = 0;

    private bool isDataLoaded = false;
    private bool isEditMode = false;
    private bool isSupervisor = false;
    private bool isViewOnly = false;
    private bool hasBeenSaved = false;
    private bool hasAttemptedSubmit = false;

    private bool showTemplateModal = false;
    private bool showTemplateUpdateWarning = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadAuthStateFromSession();
            
            if (AuthState.CurrentUser != null)
            {
                isSupervisor = AuthState.CurrentUser.RoleId == 1;
                userPositionId = AuthState.CurrentUser.PositionId;

                await InitializeFormData();
                
                isDataLoaded = true;
                StateHasChanged();
            }
        }
    }

    private async Task LoadAuthStateFromSession()
    {
        try
        {
            var userJson = await JS.InvokeAsync<string>("sessionStorage.getItem", "UserSession");
            if (!string.IsNullOrEmpty(userJson))
            {
                var user = JsonSerializer.Deserialize<User>(userJson);
                if (user != null)
                {
                    AuthState.SetAuthenticationState(user);
                    return;
                }
            }
            NavigationManager.NavigateTo("/login", true);
        }
        catch { NavigationManager.NavigateTo("/login", true); }
    }

    private async Task InitializeFormData()
    {
        allTemplates = await TemplateService.GetTemplateByPositionId(userPositionId) ?? new();
        
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var query = QueryHelpers.ParseQuery(uri.Query);

        if (query.TryGetValue("surveyId", out var id))
        {
            await LoadExistingSurvey(id!);
        }
        else
        {
            await PrepareNewSurvey();
        }

        DetermineViewOnlyMode();
    }

    private async Task PrepareNewSurvey()
    {
        isEditMode = false;
        hasBeenSaved = false;
        
        using var context = await DbContextFactory.CreateDbContextAsync();
        reqDto.DocumentId = await IdGeneratorUtil.GenerateSurveyId(context);
        
        if (AuthState.CurrentUser != null)
        {
            reqDto.RequesterId = AuthState.CurrentUser.Id;
            await LoadRequesterData();
        }
    }

    private async Task LoadExistingSurvey(string id)
    {
        isEditMode = true;
        hasBeenSaved = true;
        currentSurveyId = id;
        
        var data = await SurveyService.GetSurveyHeaderById(id);
        if (data == null) return;

        reqDto = new SurveyHeaderRequestDTO
        {
            DocumentId = data.DocumentId,
            RequesterId = data.RequesterId,
            Status = Enum.Parse<StatusType>(data.Status),
            TemplateHeaderId = data.TemplateHeaderId,
            UpdatedAtTemplate = data.UpdatedAtTemplate,
            SurveyItems = data.SurveyItems.Select(si => new SurveyItemRequestDTO
            {
                Id = si.Id,
                DocumentSurveyId = si.DocumentSurveyId,
                TemplateItemId = si.TemplateItemId,
                Question = si.Question,
                Type = Enum.Parse<QuestionType>(si.Type),
                OrderNo = si.OrderNo,
                Answer = si.Answer ?? string.Empty, // Pastikan tidak null
                CheckBox = si.CheckBox.Select(cb => new SurveyItemDetailRequestDTO
                {
                    Id = cb.Id,
                    DocumentItemId = cb.DocumentItemId,
                    TemplateItemDetailId = cb.TemplateItemDetailId,
                    Item = cb.Item,
                    IsChecked = cb.IsChecked
                }).ToList()
            }).ToList()
        };
        
        selectedTemplateName = data.Header?.TemplateName ?? "N/A";
        selectedTemplateTheme = data.Header?.Theme ?? "N/A";
        
        await LoadRequesterData();

        if (data.Header != null && data.UpdatedAtTemplate != data.Header.UpdatedAt && 
            !isSupervisor && reqDto.Status == StatusType.Draft && !isViewOnly)
        {
            showTemplateUpdateWarning = true;
        }
    }

    private async Task SelectTemplate(TemplateHeaderResponseDTO template)
    {
        var fullTemplate = await TemplateService.GetTemplateByHeaderId(template.Id);
        if (fullTemplate == null) return;

        reqDto.TemplateHeaderId = fullTemplate.Id;
        reqDto.UpdatedAtTemplate = fullTemplate.UpdatedAt;
        selectedTemplateName = fullTemplate.TemplateName;
        selectedTemplateTheme = fullTemplate.Theme;
        
        reqDto.SurveyItems = fullTemplate.Items.Select(i => new SurveyItemRequestDTO
        {
            TemplateItemId = i.Id,
            Question = i.Question,
            Type = Enum.Parse<QuestionType>(i.Type),
            OrderNo = i.OrderNo,
            Answer = string.Empty, // Inisialisasi dengan string kosong
            CheckBox = i.ItemDetails.Select(d => new SurveyItemDetailRequestDTO
            {
                TemplateItemDetailId = d.Id,
                Item = d.Item,
                IsChecked = false
            }).ToList()
        }).ToList();
        
        showTemplateModal = false;
        showTemplateUpdateWarning = false;
        alertMessage = null;
        StateHasChanged();
    }

    private async Task ShowUpdateTemplateConfirmation()
    {
        try
        {
            var result = await JS.InvokeAsync<JsonElement>("Swal.fire", new
            {
                title = "Update Template?",
                text = "Updating to the latest template will replace all current survey items. Your answers will be preserved where possible.",
                icon = "warning",
                showCancelButton = true,
                confirmButtonColor = "#4F46E5",
                cancelButtonColor = "#6b7280",
                confirmButtonText = "Yes, Update",
                cancelButtonText = "No, Keep Current"
            });

            if (result.TryGetProperty("isConfirmed", out var isConfirmed) && isConfirmed.GetBoolean())
            {
                await HandleUpdateToLatestTemplate();
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    private async Task HandleUpdateToLatestTemplate()
    {
        try
        {
            var latestTemplate = await TemplateService.GetTemplateByHeaderId(reqDto.TemplateHeaderId!);
            if (latestTemplate == null) return;

            var updatePayload = new SurveyHeaderRequestDTO 
            {
                TemplateHeaderId = latestTemplate.Id,
                UpdatedAtTemplate = latestTemplate.UpdatedAt,
                Status = reqDto.Status,
                SurveyItems = latestTemplate.Items.Select(i => new SurveyItemRequestDTO 
                {
                    TemplateItemId = i.Id,
                    Question = i.Question,
                    Type = Enum.Parse<QuestionType>(i.Type),
                    OrderNo = i.OrderNo,
                    CheckBox = i.ItemDetails.Select(d => new SurveyItemDetailRequestDTO 
                    {
                        TemplateItemDetailId = d.Id,
                        Item = d.Item
                    }).ToList()
                }).ToList()
            };

            var result = await SurveyService.UpdateSurveyHeaderFromLatestTemplate(currentSurveyId!, updatePayload);
            if (result != null)
            {
                await LoadExistingSurvey(currentSurveyId!);
                showTemplateUpdateWarning = false;
                
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Updated!",
                    text = "Template updated and answers preserved successfully!",
                    icon = "success",
                    confirmButtonColor = "#4F46E5"
                });
                StateHasChanged();
            }
        }
        catch (Exception ex) 
        { 
            await JS.InvokeVoidAsync("Swal.fire", new
            {
                title = "Error!",
                text = ex.Message,
                icon = "error",
                confirmButtonColor = "#4F46E5"
            });
        }
    }

    private async Task ValidateAndShowSubmitModal()
    {
        hasAttemptedSubmit = true;
        alertMessage = null;

        // Validasi template
        if (string.IsNullOrEmpty(reqDto.TemplateHeaderId))
        {
            await JS.InvokeVoidAsync("Swal.fire", new
            {
                title = "Validation Error",
                text = "Please select a template first",
                icon = "error",
                confirmButtonColor = "#4F46E5"
            });
            return;
        }

        // Validasi semua item
        bool hasEmptyAnswers = reqDto.SurveyItems.Any(item => 
        {
            if (item.Type == QuestionType.CheckBox)
            {
                // Cek jika ada checkbox biasa yang dicentang
                var hasCheckedRegular = item.CheckBox.Any(cb => cb.IsChecked);
                
                // Cek jika "Lainnya" aktif (Answer tidak kosong dan bukan hanya spasi)
                var isLainnyaChecked = !string.IsNullOrWhiteSpace(item.Answer);
                
                if (isLainnyaChecked)
                {
                    // Jika "Lainnya" dicentang, pastikan ada isian teksnya (bukan hanya spasi)
                    return string.IsNullOrWhiteSpace(item.Answer?.Trim());
                }
                
                // Jika tidak ada yang dicentang (baik checkbox biasa maupun "Lainnya")
                return !hasCheckedRegular && !isLainnyaChecked;
            }
            
            // Untuk tipe pertanyaan lain, validasi string kosong
            return string.IsNullOrWhiteSpace(item.Answer);
        });

        if (hasEmptyAnswers)
        {
            await JS.InvokeVoidAsync("Swal.fire", new
            {
                title = "Validation Error",
                text = "Please complete all required fields before submitting",
                icon = "error",
                confirmButtonColor = "#4F46E5"
            });
            return;
        }

        // Tampilkan konfirmasi submit
        try
        {
            var result = await JS.InvokeAsync<JsonElement>("Swal.fire", new
            {
                title = "Submit Survey?",
                text = $"Submit {reqDto.DocumentId} for approval? Once submitted, you cannot edit until reviewed.",
                icon = "question",
                showCancelButton = true,
                confirmButtonColor = "#4F46E5",
                cancelButtonColor = "#6b7280",
                confirmButtonText = "Yes, Submit",
                cancelButtonText = "Cancel"
            });

            if (result.TryGetProperty("isConfirmed", out var isConfirmed) && isConfirmed.GetBoolean())
            {
                await ProcessSave(StatusType.ConfirmToApprove);
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    private async Task HandleApprove()
    {
        try
        {
            var result = await JS.InvokeAsync<JsonElement>("Swal.fire", new
            {
                title = "Approve Survey?",
                text = $"Approve survey {reqDto.DocumentId} from {requesterData?.Username ?? "N/A"}?",
                icon = "success",
                showCancelButton = true,
                confirmButtonColor = "#22C55E",
                cancelButtonColor = "#6b7280",
                confirmButtonText = "Yes, Approve",
                cancelButtonText = "Cancel"
            });

            if (result.TryGetProperty("isConfirmed", out var isConfirmed) && isConfirmed.GetBoolean())
            {
                await UpdateStatus("Confirmed", "Survey has been approved successfully!");
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    private async Task HandleReject()
    {
        try
        {
            var result = await JS.InvokeAsync<JsonElement>("Swal.fire", new
            {
                title = "Reject Survey?",
                text = $"Reject survey {reqDto.DocumentId} from {requesterData?.Username ?? "N/A"}?",
                icon = "warning",
                showCancelButton = true,
                confirmButtonColor = "#ef4444",
                cancelButtonColor = "#6b7280",
                confirmButtonText = "Yes, Reject",
                cancelButtonText = "Cancel"
            });

            if (result.TryGetProperty("isConfirmed", out var isConfirmed) && isConfirmed.GetBoolean())
            {
                await UpdateStatus("Rejected", "Survey has been rejected!");
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    private async Task UpdateStatus(string status, string alertMsg)
    {
        try
        {
            var result = await SurveyService.UpdateSurveyHeaderStatus(currentSurveyId!, status);
            if (result != null)
            {
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Success!",
                    text = alertMsg,
                    icon = "success",
                    confirmButtonColor = "#4F46E5"
                });
                await Task.Delay(1500);
                NavigationManager.NavigateTo("/surveys");
            }
        }
        catch (Exception ex) 
        { 
            await JS.InvokeVoidAsync("Swal.fire", new
            {
                title = "Error!",
                text = ex.Message,
                icon = "error",
                confirmButtonColor = "#4F46E5"
            });
        }
    }

    private async Task HandleSaveDraft() => await ProcessSave(StatusType.Draft);

    private async Task ProcessSave(StatusType status)
    {
        if (isViewOnly) return;
        try
        {
            if (string.IsNullOrEmpty(reqDto.TemplateHeaderId))
            {
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Error!",
                    text = "Please select a template first",
                    icon = "error",
                    confirmButtonColor = "#4F46E5"
                });
                return;
            }

            reqDto.Status = status;
            if (isEditMode) await SurveyService.UpdateSurveyHeader(currentSurveyId!, reqDto);
            else await SurveyService.CreateSurveyHeader(reqDto);
            
            hasBeenSaved = true;
            var message = status == StatusType.Draft ? "Draft saved successfully!" : "Survey submitted successfully!";
            
            await JS.InvokeVoidAsync("Swal.fire", new
            {
                title = "Success!",
                text = message,
                icon = "success",
                confirmButtonColor = "#4F46E5"
            });
            await Task.Delay(1500);
            NavigationManager.NavigateTo("/surveys");
        }
        catch (Exception ex) 
        { 
            await JS.InvokeVoidAsync("Swal.fire", new
            {
                title = "Error!",
                text = ex.Message,
                icon = "error",
                confirmButtonColor = "#4F46E5"
            });
        }
    }

    private async Task HandleDelete()
    {
        if (isViewOnly) return;
        
        try
        {
            var result = await JS.InvokeAsync<JsonElement>("Swal.fire", new
            {
                title = "Delete Survey?",
                text = $"Delete survey {reqDto.DocumentId}? This action cannot be undone!",
                icon = "warning",
                showCancelButton = true,
                confirmButtonColor = "#ef4444",
                cancelButtonColor = "#6b7280",
                confirmButtonText = "Yes, Delete",
                cancelButtonText = "Cancel"
            });

            if (result.TryGetProperty("isConfirmed", out var isConfirmed) && isConfirmed.GetBoolean())
            {
                await SurveyService.DeleteSurveyHeader(currentSurveyId!);
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Deleted!",
                    text = "Survey deleted successfully!",
                    icon = "success",
                    confirmButtonColor = "#4F46E5"
                });
                await Task.Delay(1000);
                NavigationManager.NavigateTo("/surveys");
            }
        }
        catch (Exception ex) 
        { 
            await JS.InvokeVoidAsync("Swal.fire", new
            {
                title = "Error!",
                text = ex.Message,
                icon = "error",
                confirmButtonColor = "#4F46E5"
            });
        }
    }

    private void DetermineViewOnlyMode()
    {
        if (isSupervisor) isViewOnly = true;
        else if (isEditMode) isViewOnly = reqDto.Status != StatusType.Draft;
        else isViewOnly = false;
    }

    private async Task LoadRequesterData()
    {
        if (!string.IsNullOrEmpty(reqDto.RequesterId))
            requesterData = await UserService.GetUserById(reqDto.RequesterId);
    }

    private string GetPageTitle() => isSupervisor ? "Review Survey" : (isViewOnly ? "View Survey" : (isEditMode ? "Edit Survey" : "Create New Survey"));
    private string GetPageSubtitle() => isSupervisor ? $"Review: {reqDto.DocumentId}" : (isViewOnly ? $"View: {reqDto.DocumentId}" : (isEditMode ? $"Modify: {reqDto.DocumentId}" : "Fill out survey form"));
    private string GetCurrentStatus() => (!hasBeenSaved && !isEditMode) ? "New" : reqDto.Status.ToString();
    private string GetStatusDisplay(string status) => status == "ConfirmToApprove" ? "Pending Approval" : status;
    private string GetStatusBadgeClass(string status) => status switch
    {
        "New" => "bg-secondary", 
        "Draft" => "bg-warning text-dark", 
        "ConfirmToApprove" => "bg-info text-dark", 
        "Confirmed" => "badge-success-custom", 
        "Rejected" => "badge-danger-custom", 
        _ => "bg-secondary"
    };
}