using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using TaskSurvey.Infrastructure.DTOs.UserDTOs;
using TaskSurvey.Infrastructure.Models;
using TaskSurvey.Infrastructure.Services.IServices;
using TaskSurvey.StateServices;

namespace TaskSurvey.Components.Pages.Profile;

public partial class Profile : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private AuthState AuthState { get; set; } = default!;
    [Inject] private IUserService UserService { get; set; } = default!;

    private User? currentUser;
    private UserResponseDTO? supervisor;
    private bool isSupervisor = false;
    private bool isUser = false;
    private bool isOverseer = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadAuthStateFromSession();
            await LoadProfileData();
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
                    currentUser = user;
                    isSupervisor = user.RoleId == 1;
                    isUser = user.RoleId == 2;
                    isOverseer = user.RoleId == 3;
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

    private async Task LoadProfileData()
    {
        try
        {
            if (currentUser?.Id != null)
            {
                var userData = await UserService.GetUserById(currentUser.Id);
                
                if (userData != null)
                {
                    currentUser = new User
                    {
                        Id = userData.Id,
                        Username = userData.Username,
                        PasswordHash = "",
                        PositionId = userData.PositionId,
                        PositionName = userData.PositionName,
                        RoleId = userData.RoleId,
                        Role = new Role { RoleName = userData.Role?.RoleName! },
                        Position = new Position { PositionLevel = userData.Position?.PositionLevel! }
                    };

                    if (isUser && userData.Supervisor != null)
                    {
                        supervisor = userData.Supervisor;
                    }
                    
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading profile data: {ex.Message}");
        }
    }

    private async Task HandleLogout()
    {
        try
        {
            var confirmed = await JS.InvokeAsync<bool>("Swal.fire", new
            {
                title = "Sign Out?",
                text = "Are you sure you want to sign out?",
                icon = "warning",
                showCancelButton = true,
                confirmButtonColor = "#4F46E5",
                cancelButtonColor = "#64748B",
                confirmButtonText = "Yes, sign out",
                cancelButtonText = "Cancel"
            });

            var result = await JS.InvokeAsync<JsonElement>("Swal.fire", new
            {
                title = "Sign Out?",
                text = "Are you sure you want to sign out?",
                icon = "warning",
                showCancelButton = true,
                confirmButtonColor = "#4F46E5",
                cancelButtonColor = "#64748B",
                confirmButtonText = "Yes, sign out",
                cancelButtonText = "Cancel"
            });

            if (result.TryGetProperty("isConfirmed", out var isConfirmedElement) && isConfirmedElement.GetBoolean())
            {
                await JS.InvokeVoidAsync("sessionStorage.removeItem", "UserSession");
                AuthState.Logout();

                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    icon = "success",
                    title = "Signed Out!",
                    text = "You have been signed out successfully.",
                    timer = 1500,
                    showConfirmButton = false
                });

                await Task.Delay(1000);
                NavigationManager.NavigateTo("/login", true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during logout: {ex.Message}");
            await JS.InvokeVoidAsync("sessionStorage.removeItem", "UserSession");
            AuthState.Logout();
            NavigationManager.NavigateTo("/login", true);
        }
    }
}