using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Text.Json;
using TaskSurvey.Infrastructure.Data;
using TaskSurvey.Infrastructure.DTOs.PositionDTOs;
using TaskSurvey.Infrastructure.DTOs.UserDTOs;
using TaskSurvey.Infrastructure.Models;
using TaskSurvey.Infrastructure.Services.IServices;
using TaskSurvey.Infrastructure.Utils;
using TaskSurvey.StateServices;

namespace TaskSurvey.Components.Pages.FormUser;

public partial class FormUser : ComponentBase
{
    [Inject] private IUserService UserService { get; set; } = default!;
    [Inject] private IPositionService PositionService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private AuthState AuthState { get; set; } = default!;
    [Inject] private IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private UserRequestDTO reqDto = new();
    private List<PositionResponseDTO> positions = new();
    private List<UserResponseDTO> allUsersList = new();
    
    private string displayId = "";
    private string? currentUserId;
    private bool isEditMode = false;
    private bool showSupLookup = false;
    private bool showUserLookup = false;
    private bool hasAttemptedSubmit = false;
    private string searchQuery = "";
    private string? alertMessage;
    private string? successMessage;
    private string selectedSupName = "";
    private string selectedSupPos = "";
    private bool isUser = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadAuthStateFromSession();
            
            isUser = AuthState.CurrentUser?.RoleId == 2;
            if (isUser)
            {
                NavigationManager.NavigateTo("/dashboard", true);
                return;
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

    protected override async Task OnInitializedAsync()
    {
        positions = await PositionService.GetPositions();
        allUsersList = await UserService.GetUsers();

        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("userId", out var id))
        {
            await SelectUserToEdit(id!);
        }
        else
        {
            await GenerateNextId();
        }
    }

    private string GetValidationClass(string? value) => 
        hasAttemptedSubmit && string.IsNullOrWhiteSpace(value) ? "is-invalid" : "";

    private async Task GenerateNextId()
    {
        using var context = await DbContextFactory.CreateDbContextAsync();
        displayId = await IdGeneratorUtil.GetNextFormattedUserId(context, false);
    }

    private void OpenUserLookupModal()
    {
        showUserLookup = true;
        searchQuery = "";
    }

    private void OpenSupervisorLookupModal()
    {
        showSupLookup = true;
        searchQuery = "";
    }

    private async Task SelectUserToEdit(string id)
    {
        CloseModals();
        currentUserId = id;
        isEditMode = true;
        displayId = id;
        
        var user = await UserService.GetUserById(id);
        if (user != null)
        {
            reqDto.Username = user.Username;
            reqDto.PositionId = user.PositionId; 
            reqDto.PositionName = user.PositionName;
            reqDto.SupervisorId = user.Supervisor?.Id;
            selectedSupName = user.Supervisor?.Username ?? "";
            selectedSupPos = user.Supervisor?.PositionName ?? "";
        }
        StateHasChanged();
    }

    private void SelectSup(UserResponseDTO s)
    {
        reqDto.SupervisorId = s.Id;
        selectedSupName = s.Username;
        selectedSupPos = s.PositionName;
        CloseModals();
    }

    private void CloseModals() 
    { 
        showSupLookup = false; 
        showUserLookup = false; 
        searchQuery = ""; 
    }

    private async Task ShowSubmitConfirmation()
    {
        hasAttemptedSubmit = true;
        alertMessage = null;

        if (string.IsNullOrWhiteSpace(reqDto.Username) || 
            reqDto.PositionId == 0 || 
            string.IsNullOrWhiteSpace(reqDto.PositionName) || 
            string.IsNullOrWhiteSpace(reqDto.SupervisorId))
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

        try
        {
            var result = await JS.InvokeAsync<JsonElement>("Swal.fire", new
            {
                title = $"{(isEditMode ? "Update" : "Create")} User?",
                html = $@"
                    <div style='text-align: left; padding: 10px;'>
                        <p style='margin-bottom: 15px;'>Are you sure you want to {(isEditMode ? "update" : "create")} this user?</p>
                        <div style='background: #f8f9fa; padding: 15px; border-radius: 8px;'>
                            <p style='margin: 5px 0; font-size: 0.9rem;'><strong>ID:</strong> {displayId}</p>
                            <p style='margin: 5px 0; font-size: 0.9rem;'><strong>Username:</strong> {reqDto.Username}</p>
                            <p style='margin: 5px 0; font-size: 0.9rem;'><strong>Position:</strong> {reqDto.PositionName}</p>
                            <p style='margin: 5px 0; font-size: 0.9rem;'><strong>Supervisor:</strong> {selectedSupName}</p>
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
                await HandleSubmit();
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    private async Task HandleSubmit()
    {
        alertMessage = null;
        successMessage = null;

        try 
        {
            var selectedSupervisor = allUsersList.FirstOrDefault(u => u.Id == reqDto.SupervisorId);

            if (selectedSupervisor != null)
            {
                if (selectedSupervisor.Role?.Id == 1)
                {
                    reqDto.RoleId = 3;
                }
                else if (selectedSupervisor.Role?.Id == 3)
                {
                    reqDto.RoleId = 2;
                }
                else 
                {
                    reqDto.RoleId = 2; 
                }
            }
            else
            {
                reqDto.RoleId = 2; 
            }

            reqDto.PasswordHash = ""; 

            if (isEditMode)
            {
                await UserService.UpdateUser(currentUserId!, reqDto, reqDto.SupervisorId);
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Updated!",
                    text = "User updated successfully!",
                    icon = "success",
                    confirmButtonColor = "#4F46E5"
                });
            }
            else
            {
                await UserService.CreateUser(reqDto);
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Created!",
                    text = "User created successfully!",
                    icon = "success",
                    confirmButtonColor = "#4F46E5"
                });
            }
            
            await Task.Delay(1500);
            NavigationManager.NavigateTo("/users");
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
        currentUserId = null;
        reqDto = new UserRequestDTO();
        selectedSupName = "";
        selectedSupPos = "";
        await GenerateNextId();
        NavigationManager.NavigateTo("/users/form", true);
    }

    private async Task ShowDeleteConfirmation()
    {
        try
        {
            var result = await JS.InvokeAsync<JsonElement>("Swal.fire", new
            {
                title = "Delete User?",
                html = $@"
                    <div style='text-align: left; padding: 10px;'>
                        <p style='margin-bottom: 15px;'>Are you sure you want to delete this user?</p>
                        <div style='background: #fee2e2; padding: 15px; border-radius: 8px; border: 1px solid #ef4444;'>
                            <p style='margin: 0 0 10px 0; color: #dc2626; font-weight: 600;'>
                                ⚠️ This action cannot be undone!
                            </p>
                            <p style='margin: 5px 0; font-size: 0.9rem;'><strong>ID:</strong> {displayId}</p>
                            <p style='margin: 5px 0; font-size: 0.9rem;'><strong>Username:</strong> {reqDto.Username}</p>
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
                await HandleDelete();
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    private async Task HandleDelete()
    {
        try
        {
            await UserService.DeleteUser(currentUserId!);
            await JS.InvokeVoidAsync("Swal.fire", new
            {
                title = "Deleted!",
                text = "User deleted successfully!",
                icon = "success",
                confirmButtonColor = "#4F46E5"
            });
            await Task.Delay(1000);
            NavigationManager.NavigateTo("/users");
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
}