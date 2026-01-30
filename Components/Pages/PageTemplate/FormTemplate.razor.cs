using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Text.Json;
using TaskSurvey.Infrastructure.Data;
using TaskSurvey.Infrastructure.DTOs.PositionDTOs;
using TaskSurvey.Infrastructure.DTOs.SurveyDTOs;
using TaskSurvey.Infrastructure.DTOs.TemplateDTOs;
using TaskSurvey.Infrastructure.Models;
using TaskSurvey.Infrastructure.Services.IServices;
using TaskSurvey.Infrastructure.Utils;
using TaskSurvey.StateServices;

namespace TaskSurvey.Components.Pages.FormTemplate;

public partial class FormTemplate : ComponentBase
{
    [Inject] private ITemplateService TemplateService { get; set; } = default!;
    [Inject] private IPositionService PositionService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private AuthState AuthState { get; set; } = default!;
    [Inject] private IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private TemplateHeaderRequestDTO reqDto = new() { Items = new() };
    private List<PositionResponseDTO> positions = new();
    private List<TemplateHeaderResponseDTO> allTemplates = new();
    private TemplateItemRequestDTO currentItem = new();
    private TemplateItemRequestDTO? _editingItemReference = null;
    
    private bool isEditItem = false;
    private bool isEditMode = false;
    private bool showItemModal = false;
    private bool showLookupModal = false;
    private bool hasAttemptedSubmit = false;
    private bool itemModalAttemptedSave = false;
    
    private string searchQuery = "";
    private string? alertMessage;
    private string? successMessage;
    private int userPositionId = 0;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadAuthStateFromSession();
            
            userPositionId = AuthState.CurrentUser?.PositionId ?? 0;
            
            if (!isEditMode) 
            {
                reqDto.PositionId = userPositionId;
            }

            positions = await PositionService.GetPositions();
            allTemplates = await TemplateService.GetTemplateHeaders();

            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("templateId", out var id))
            {
                await SelectTemplate(id!);
            }
            else
            {
                await GenerateNewId();
            }

            StateHasChanged();
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
        catch (Exception)
        {
            NavigationManager.NavigateTo("/login", true);
        }
    }

    private string GetValidationClass(string? value) => 
        hasAttemptedSubmit && string.IsNullOrWhiteSpace(value) ? "is-invalid" : "";

    private string GetQuestionTypeDisplay(QuestionType type) => type switch
    {
        QuestionType.TextBox => "Text Box",
        QuestionType.TextArea => "Text Area",
        QuestionType.Number => "Number",
        QuestionType.CheckBox => "Checkbox",
        _ => type.ToString()
    };

    private string GetQuestionTypeDescription(QuestionType type) => type switch
    {
        QuestionType.TextBox => "Single line text response",
        QuestionType.TextArea => "Multi-line text response for longer answers",
        QuestionType.Number => "Numeric value only",
        QuestionType.CheckBox => "Multiple options selection with checkboxes",
        _ => ""
    };

    private async Task GenerateNewId()
    {
        using var context = await DbContextFactory.CreateDbContextAsync();
        reqDto.Id = await IdGeneratorUtil.GenerateTemplateId(context);
    }

    private void OpenLookupModal()
    {
        showLookupModal = true;
        searchQuery = "";
    }

    private async Task SelectTemplate(string id)
    {
        var data = await TemplateService.GetTemplateByHeaderId(id);
        if (data != null)
        {
            isEditMode = true;
            reqDto.Id = data.Id;
            reqDto.TemplateName = data.TemplateName;
            reqDto.PositionId = data.PositionId;
            reqDto.Theme = data.Theme;
            
            reqDto.Items = data.Items.Select(i => new TemplateItemRequestDTO {
                Id = i.Id,
                Question = i.Question, 
                Type = Enum.Parse<QuestionType>(i.Type, true), 
                OrderNo = i.OrderNo,
                ItemDetails = i.ItemDetails.Select(d => new TemplateItemDetailRequestDTO { 
                    Id = d.Id,
                    Item = d.Item 
                }).ToList()
            }).ToList();
        }
        showLookupModal = false;
        StateHasChanged();
    }

    private void PrepareNewItem()
    {
        isEditItem = false;
        itemModalAttemptedSave = false;
        currentItem = new TemplateItemRequestDTO { 
            Type = QuestionType.TextBox, 
            ItemDetails = new() 
        };
        showItemModal = true;
    }

    private void OnTypeChanged()
    {
        if (currentItem.Type == QuestionType.CheckBox)
        {
            if (!currentItem.ItemDetails.Any())
            {
                AddDetailRow();
            }
        }
    }

    private void SaveItemToTable()
    {
        itemModalAttemptedSave = true;
        
        if (string.IsNullOrWhiteSpace(currentItem.Question))
        {
            alertMessage = "Please enter a question before saving.";
            return;
        }

        if (currentItem.Type == QuestionType.CheckBox && !currentItem.ItemDetails.Any())
        {
            alertMessage = "Please add at least one option for checkbox type.";
            return;
        }

        if (isEditItem && _editingItemReference != null)
        {
            _editingItemReference.Question = currentItem.Question;
            _editingItemReference.Type = currentItem.Type;
            _editingItemReference.ItemDetails = currentItem.ItemDetails;
        }
        else
        {
            currentItem.OrderNo = reqDto.Items.Any() ? reqDto.Items.Max(x => x.OrderNo) + 1 : 1;
            reqDto.Items.Add(currentItem);
        }
        
        CloseItemModal();
    }

    private void CloseItemModal()
    {
        showItemModal = false;
        itemModalAttemptedSave = false;
        alertMessage = null;
    }

    private void EditItem(TemplateItemRequestDTO item) 
    { 
        isEditItem = true;
        itemModalAttemptedSave = false;
        currentItem = new TemplateItemRequestDTO 
        {
            Id = item.Id,
            Question = item.Question,
            Type = item.Type,
            OrderNo = item.OrderNo,
            ItemDetails = item.ItemDetails.Select(d => new TemplateItemDetailRequestDTO { 
                Id = d.Id, 
                Item = d.Item 
            }).ToList()
        };
        _editingItemReference = item; 
        showItemModal = true; 
    }

    private void MoveUp(TemplateItemRequestDTO item)
    {
        var list = reqDto.Items.OrderBy(x => x.OrderNo).ToList();
        int idx = list.IndexOf(item);
        if (idx > 0)
        {
            var prev = list[idx - 1];
            int temp = item.OrderNo;
            item.OrderNo = prev.OrderNo;
            prev.OrderNo = temp;
        }
    }

    private void MoveDown(TemplateItemRequestDTO item)
    {
        var list = reqDto.Items.OrderBy(x => x.OrderNo).ToList();
        int idx = list.IndexOf(item);
        if (idx < list.Count - 1)
        {
            var next = list[idx + 1];
            int temp = item.OrderNo;
            item.OrderNo = next.OrderNo;
            next.OrderNo = temp;
        }
    }

    private async Task ShowRemoveItemConfirmation(TemplateItemRequestDTO item)
    {
        try
        {
            var result = await JS.InvokeAsync<JsonElement>("Swal.fire", new
            {
                title = "Remove Item?",
                text = $"Remove '{item.Question}' from template?",
                icon = "warning",
                showCancelButton = true,
                confirmButtonColor = "#ef4444",
                cancelButtonColor = "#6b7280",
                confirmButtonText = "Yes, Remove",
                cancelButtonText = "Cancel"
            });

            if (result.TryGetProperty("isConfirmed", out var isConfirmed) && isConfirmed.GetBoolean())
            {
                reqDto.Items.Remove(item);
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    private void AddDetailRow() => currentItem.ItemDetails.Add(new TemplateItemDetailRequestDTO());
    
    private void RemoveDetailRow(TemplateItemDetailRequestDTO det) => currentItem.ItemDetails.Remove(det);

    private async Task ShowSubmitConfirmation()
    {
        hasAttemptedSubmit = true;
        alertMessage = null;

        if (string.IsNullOrWhiteSpace(reqDto.TemplateName) || reqDto.PositionId == 0)
        {
            await JS.InvokeVoidAsync("Swal.fire", new
            {
                title = "Validation Error",
                text = "Please complete all required fields before submitting.",
                icon = "error",
                confirmButtonColor = "#4F46E5"
            });
            return;
        }

        if (!reqDto.Items.Any())
        {
            await JS.InvokeVoidAsync("Swal.fire", new
            {
                title = "Validation Error",
                text = "Please add at least one survey item before submitting.",
                icon = "error",
                confirmButtonColor = "#4F46E5"
            });
            return;
        }

        var positionName = positions.FirstOrDefault(p => p.Id == reqDto.PositionId)?.PositionLevel ?? "-";
        
        try
        {
            var result = await JS.InvokeAsync<JsonElement>("Swal.fire", new
            {
                title = $"{(isEditMode ? "Update" : "Submit")} Template?",
                html = $@"
                    <div style='text-align: left; padding: 10px;'>
                        <p style='margin-bottom: 15px;'>Are you sure you want to {(isEditMode ? "update" : "create")} this template?</p>
                        <div style='background: #f8f9fa; padding: 15px; border-radius: 8px;'>
                            <p style='margin: 5px 0; font-size: 0.9rem;'><strong>ID:</strong> {reqDto.Id}</p>
                            <p style='margin: 5px 0; font-size: 0.9rem;'><strong>Name:</strong> {reqDto.TemplateName}</p>
                            <p style='margin: 5px 0; font-size: 0.9rem;'><strong>Position:</strong> {positionName}</p>
                            <p style='margin: 5px 0; font-size: 0.9rem;'><strong>Items:</strong> {reqDto.Items.Count} item(s)</p>
                        </div>
                    </div>
                ",
                icon = "question",
                showCancelButton = true,
                confirmButtonColor = "#4F46E5",
                cancelButtonColor = "#6b7280",
                confirmButtonText = $"Yes, {(isEditMode ? "Update" : "Submit")}",
                cancelButtonText = "Cancel"
            });

            if (result.TryGetProperty("isConfirmed", out var isConfirmed) && isConfirmed.GetBoolean())
            {
                await SaveAll();
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    private async Task SaveAll()
    {
        alertMessage = null;
        successMessage = null;

        try 
        {
            if (isEditMode) 
            {
                await TemplateService.UpdateTemplate(reqDto.Id!, reqDto);
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Updated!",
                    text = "Template updated successfully!",
                    icon = "success",
                    confirmButtonColor = "#4F46E5"
                });
            }
            else 
            {
                await TemplateService.CreateTemplate(reqDto);
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Created!",
                    text = "Template created successfully!",
                    icon = "success",
                    confirmButtonColor = "#4F46E5"
                });
            }
            
            await Task.Delay(1500);
            NavigationManager.NavigateTo("/templates");
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

    private async Task ShowDeleteConfirmation()
    {
        try
        {
            var result = await JS.InvokeAsync<JsonElement>("Swal.fire", new
            {
                title = "Delete Template?",
                html = $@"
                    <div style='text-align: left; padding: 10px;'>
                        <p style='margin-bottom: 15px;'>Are you sure you want to delete this template?</p>
                        <div style='background: #fee2e2; padding: 15px; border-radius: 8px; border: 1px solid #ef4444;'>
                            <p style='margin: 0 0 10px 0; color: #dc2626; font-weight: 600;'>
                                ⚠️ This action cannot be undone!
                            </p>
                            <p style='margin: 5px 0; font-size: 0.9rem;'><strong>ID:</strong> {reqDto.Id}</p>
                            <p style='margin: 5px 0; font-size: 0.9rem;'><strong>Name:</strong> {reqDto.TemplateName}</p>
                            <p style='margin: 5px 0; font-size: 0.9rem;'><strong>Items:</strong> {reqDto.Items.Count} item(s) will be deleted</p>
                        </div>
                    </div>
                ",
                icon = "warning",
                showCancelButton = true,
                confirmButtonColor = "#ef4444",
                cancelButtonColor = "#6b7280",
                confirmButtonText = "Yes, Delete",
                cancelButtonText = "Cancel"
            });

            if (result.TryGetProperty("isConfirmed", out var isConfirmed) && isConfirmed.GetBoolean())
            {
                await DeleteCurrentTemplate();
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    private async Task DeleteCurrentTemplate()
    {
        try 
        {
            var success = await TemplateService.DeleteTemplate(reqDto.Id!);
            if (success)
            {
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Deleted!",
                    text = "Template deleted successfully!",
                    icon = "success",
                    confirmButtonColor = "#4F46E5"
                });
                await Task.Delay(1000);
                NavigationManager.NavigateTo("/templates");
            }
            else
            {
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Error!",
                    text = "Failed to delete template.",
                    icon = "error",
                    confirmButtonColor = "#4F46E5"
                });
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

    private async Task ResetForm()
    {
        isEditMode = false;
        hasAttemptedSubmit = false;
        alertMessage = null;
        successMessage = null;
        reqDto = new() { Items = new() };
        reqDto.PositionId = userPositionId;
        await GenerateNewId();
        NavigationManager.NavigateTo("/templates/form", true);
    }
}