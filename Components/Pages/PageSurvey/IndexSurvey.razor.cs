using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using TaskSurvey.Infrastructure.DTOs.SurveyDTOs;
using TaskSurvey.Infrastructure.Models;
using TaskSurvey.Infrastructure.Services.IServices;

namespace TaskSurvey.Components.Pages.IndexSurvey
{
    public partial class IndexSurvey : ComponentBase
    {
        [Inject] private ISurveyService SurveyService { get; set; } = null!;
        [Inject] private IUserService UserService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private TaskSurvey.StateServices.AuthState AuthState { get; set; } = null!;
        [Inject] private IJSRuntime JS { get; set; } = null!;

        private List<SurveyHeaderResponseDTO> surveys = new();
        private List<SurveyHeaderResponseDTO> filteredSurveys = new();
        private List<string> supervisedUserIds = new();
        
        private string searchQuery = "";
        private string selectedStatus = "";
        private bool isSupervisor = false;
        private bool isDataLoaded = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadAuthStateFromSession();
                
                if (AuthState.CurrentUser != null)
                {
                    isSupervisor = AuthState.CurrentUser.RoleId == 1;
                    
                    await InitializePageData();
                    
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

        private async Task InitializePageData()
        {
            if (isSupervisor)
            {
                await LoadSupervisedUsers();
            }
            await LoadData();
        }

        private async Task LoadSupervisedUsers()
        {
            try
            {
                var allUsers = await UserService.GetUsers();
                var currentUserId = AuthState.CurrentUser?.Id;
                
                supervisedUserIds = allUsers
                    .Where(u => u.Supervisor != null && u.Supervisor.Id == currentUserId)
                    .Select(u => u.Id)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading supervised users: {ex.Message}");
            }
        }

        private async Task LoadData()
        {
            try
            {
                if (isSupervisor)
                {
                    var tasks = supervisedUserIds.Select(id => 
                        SurveyService.GetDocumentSurveyForSupervisor(id, "ConfirmToApprove"));
                    
                    var results = await Task.WhenAll(tasks);
                    surveys = results.Where(r => r != null).SelectMany(r => r!).ToList();
                }
                else if (AuthState.CurrentUser != null)
                {
                    surveys = await SurveyService.GetSurveyHeaderByUserId(AuthState.CurrentUser.Id) ?? new();
                }
                
                ApplyFilters();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading survey data: {ex.Message}");
            }
        }

        private void OnSearchInput(ChangeEventArgs e)
        {
            searchQuery = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        private void OnStatusFilterChanged(ChangeEventArgs e)
        {
            selectedStatus = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = surveys.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var q = searchQuery.Trim().ToLower();
                query = query.Where(s => 
                    s.DocumentId.ToLower().Contains(q) || 
                    (s.Requester?.Username?.ToLower().Contains(q) ?? false));
            }

            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = query.Where(s => s.Status == selectedStatus);
            }

            filteredSurveys = query.OrderByDescending(s => s.CreatedAt).ToList();
        }

        private void ViewSurvey(string id) => NavigationManager.NavigateTo($"/surveys/form?surveyId={id}");

        private string GetStatusBadgeClass(string status) => status switch
        {
            "Draft" => "bg-warning text-dark",
            "ConfirmToApprove" => "bg-info text-dark",
            "Confirmed" => "badge-success-custom",
            "Rejected" => "badge-danger-custom",
            _ => "bg-secondary"
        };

        private string GetStatusDisplay(string status) => status == "ConfirmToApprove" ? "Pending Approval" : status;
    }
}