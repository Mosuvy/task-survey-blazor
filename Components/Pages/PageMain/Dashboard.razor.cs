using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using TaskSurvey.Infrastructure.Models;
using TaskSurvey.Infrastructure.Services.IServices;
using TaskSurvey.StateServices;

namespace TaskSurvey.Components.Pages.Dashboard;

public partial class Dashboard : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private AuthState AuthState { get; set; } = default!;
    [Inject] private IUserService UserService { get; set; } = default!;
    [Inject] private ITemplateService TemplateService { get; set; } = default!;
    [Inject] private ISurveyService SurveyService { get; set; } = default!;

    private User? currentUser;
    private bool isSupervisor = false;
    private bool isUser = false;

    private int totalSurveys = 0;
    private int totalTemplates = 0;
    private int totalUsers = 0;
    private int pendingSurveys = 0;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadAuthStateFromSession();
            await LoadDashboardData();
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

    private async Task LoadDashboardData()
    {
        try
        {
            var surveys = await SurveyService.GetSurveyHeaders();
            totalSurveys = surveys?.Count ?? 0;
            pendingSurveys = surveys?.Count(s => s.Status?.ToLower() == "confirmtoapprove") ?? 0;

            var templates = await TemplateService.GetTemplateByPositionId(currentUser?.PositionId ?? 0);
            totalTemplates = templates?.Count ?? 0;

            var users = await UserService.GetUsers();
            totalUsers = users?.Count ?? 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading dashboard data: {ex.Message}");
        }
    }

    private string GetGreeting()
    {
        var wibTime = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
        );

        var hour = wibTime.Hour;

        return hour switch
        {
            >= 0 and < 11 => "Selamat Pagi",
            >= 11 and < 15 => "Selamat Siang",
            >= 15 and < 18 => "Selamat Sore",
            _ => "Selamat Malam"
        };
    }

    private string GetGreetingIcon()
    {
        var wibTime = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
        );

        var hour = wibTime.Hour;

        return hour switch
        {
            >= 0 and < 11 => "bi-brightness-high-fill",
            >= 11 and < 15 => "bi-sun-fill",
            >= 15 and < 18 => "bi-sunset-fill",
            _ => "bi-moon-stars-fill"
        };
    }
}