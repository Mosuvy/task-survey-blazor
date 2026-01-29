using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using TaskSurvey.Infrastructure.DTOs.AuthDTOs;
using TaskSurvey.Infrastructure.Services.IServices;
using TaskSurvey.StateServices;

namespace TaskSurvey.Components.Pages.Login;

public partial class Login : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private IAuthService AuthService { get; set; } = default!;
    [Inject] private AuthState AuthState { get; set; } = default!;

    private LoginRequestDTO loginViewModel = new();
    private bool isLoading = false;
    private bool showPassword = false;

    private void TogglePasswordVisibility()
    {
        showPassword = !showPassword;
    }

    private async Task HandleLogin()
    {
        try
        {
            isLoading = true;
            StateHasChanged();

            // Validasi
            if (string.IsNullOrWhiteSpace(loginViewModel.Username))
            {
                await ShowErrorAlert("Username is required");
                isLoading = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(loginViewModel.Password))
            {
                await ShowErrorAlert("Password is required");
                isLoading = false;
                return;
            }

            var loginDTO = new LoginRequestDTO 
            { 
                Username = loginViewModel.Username, 
                Password = loginViewModel.Password,
            };

            var result = await AuthService.Login(loginDTO);
            
            if (result != null)
            {
                AuthState.SetAuthenticationState(result);
                
                var userJson = JsonSerializer.Serialize(result);
                await JS.InvokeVoidAsync("sessionStorage.setItem", "UserSession", userJson);

                await ShowSuccessAlert("Login successful! Redirecting...");
                
                await Task.Delay(1000);

                NavigationManager.NavigateTo("/dashboard", true);
            }
            else
            {
                await ShowErrorAlert("Invalid username or password");
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAlert($"An error occurred: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task ShowSuccessAlert(string message)
    {
        try
        {
            await JS.InvokeVoidAsync("Swal.fire", new
            {
                icon = "success",
                title = "Success!",
                text = message,
                timer = 1500,
                showConfirmButton = false,
                toast = true,
                position = "top-end"
            });
        }
        catch
        {
            await JS.InvokeVoidAsync("alert", message);
        }
    }

    private async Task ShowErrorAlert(string message)
    {
        try
        {
            await JS.InvokeVoidAsync("Swal.fire", new
            {
                icon = "error",
                title = "Error!",
                text = message,
                confirmButtonText = "OK",
                confirmButtonColor = "#4F46E5"
            });
        }
        catch
        {
            await JS.InvokeVoidAsync("alert", message);
        }
    }
}