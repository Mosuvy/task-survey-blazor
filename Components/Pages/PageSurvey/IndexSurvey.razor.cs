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
        private bool isOverseer = false;
        private bool canApprove = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadAuthStateFromSession();
                
                if (AuthState.CurrentUser != null)
                {
                    isSupervisor = AuthState.CurrentUser.RoleId == 1;
                    isOverseer = AuthState.CurrentUser.RoleId == 3;

                    canApprove = isSupervisor || isOverseer;
                    
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
            if (canApprove)
            {
                await LoadSupervisedUsers();
            }
            await LoadData();
        }

        private async Task LoadSupervisedUsers()
        {
            try
            {
                var currentUserId = AuthState.CurrentUser?.Id;
                supervisedUserIds = await UserService.GetSubordinateIds(currentUserId!);
                
                Console.WriteLine($"Loaded {supervisedUserIds.Count} subordinates for user {currentUserId}");
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
                surveys = new();
                
                if (canApprove)
                {
                    if (supervisedUserIds.Count > 0)
                    {
                        var tasks = supervisedUserIds.Select(id => 
                        {
                            return SurveyService.GetDocumentSurveyForSupervisor(id, "ConfirmToApprove");
                        });
                        
                        var results = await Task.WhenAll(tasks);
                        var subordinateSurveys = results.Where(r => r != null).SelectMany(r => r!).ToList();
                        
                        surveys.AddRange(subordinateSurveys);
                    }
                    
                    var ownSurveys = await SurveyService.GetSurveyHeaderByUserId(AuthState.CurrentUser!.Id) ?? new();
                    surveys.AddRange(ownSurveys);
                }
                else
                {
                    surveys = await SurveyService.GetSurveyHeaderByUserId(AuthState.CurrentUser!.Id) ?? new();
                }
                
                surveys = surveys.GroupBy(s => s.DocumentId).Select(g => g.First()).ToList();
                
                ApplyFilters();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading survey data: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
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