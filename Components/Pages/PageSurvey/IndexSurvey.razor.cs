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
        private List<SurveyHeaderResponseDTO> draftSurveys = new();
        private List<SurveyHeaderResponseDTO> requestedSurveys = new();
        private List<SurveyHeaderResponseDTO> confirmedSurveys = new();
        private List<SurveyHeaderResponseDTO> rejectedSurveys = new();
        private List<SurveyHeaderResponseDTO> processedSurveys = new();
        private List<string> supervisedUserIds = new();
        
        private string searchQuery = "";
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
                    // For supervisors/overseers: load subordinates' surveys
                    if (supervisedUserIds.Count > 0)
                    {
                        var tasks = supervisedUserIds.Select(id => 
                            SurveyService.GetSurveyHeaderByUserId(id)
                        );
                        
                        var results = await Task.WhenAll(tasks);
                        var subordinateSurveys = results.Where(r => r != null).SelectMany(r => r!).ToList();
                        
                        surveys.AddRange(subordinateSurveys);
                    }
                    
                    // For overseers: also load their own surveys (including drafts)
                    if (isOverseer)
                    {
                        var ownSurveys = await SurveyService.GetSurveyHeaderByUserId(AuthState.CurrentUser!.Id) ?? new();
                        surveys.AddRange(ownSurveys);
                    }
                }
                else
                {
                    // For regular users: load their own surveys
                    surveys = await SurveyService.GetSurveyHeaderByUserId(AuthState.CurrentUser!.Id) ?? new();
                }
                
                // Remove duplicates
                surveys = surveys.GroupBy(s => s.DocumentId).Select(g => g.First()).ToList();
                
                // Apply filters and categorize
                ApplyFilters();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading survey data: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void ApplyFilters()
        {
            var query = surveys.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var q = searchQuery.Trim().ToLower();
                query = query.Where(s => 
                    s.DocumentId.ToLower().Contains(q) || 
                    (s.Requester?.Username?.ToLower().Contains(q) ?? false));
            }

            var filteredList = query.OrderByDescending(s => s.CreatedAt).ToList();

            // Categorize surveys based on status
            if (canApprove)
            {
                // For supervisors/overseers
                if (isOverseer)
                {
                    // Overseer can see:
                    // 1. Their own drafts
                    // 2. Subordinates' surveys pending approval
                    // 3. All confirmed/rejected surveys (both own and subordinates)
                    
                    var currentUserId = AuthState.CurrentUser?.Id;
                    
                    // Own drafts
                    draftSurveys = filteredList.Where(s => 
                        s.Status == "Draft" && 
                        s.RequesterId == currentUserId).ToList();
                    
                    // Subordinates' pending approvals (exclude own surveys)
                    requestedSurveys = filteredList.Where(s => 
                        s.Status == "ConfirmToApprove" && 
                        s.RequesterId != currentUserId).ToList();
                    
                    // All confirmed and rejected (own + subordinates)
                    confirmedSurveys = filteredList.Where(s => s.Status == "Confirmed").ToList();
                    rejectedSurveys = filteredList.Where(s => s.Status == "Rejected").ToList();
                    
                    // Processed: own requests + confirmed/rejected from all
                    processedSurveys = filteredList.Where(s => 
                        (s.Status == "ConfirmToApprove" && s.RequesterId == currentUserId) ||
                        s.Status == "Confirmed" || 
                        s.Status == "Rejected").ToList();
                }
                else
                {
                    // For pure supervisors (no own surveys)
                    requestedSurveys = filteredList.Where(s => s.Status == "ConfirmToApprove").ToList();
                    confirmedSurveys = filteredList.Where(s => s.Status == "Confirmed").ToList();
                    rejectedSurveys = filteredList.Where(s => s.Status == "Rejected").ToList();
                    processedSurveys = filteredList.Where(s => s.Status == "Confirmed" || s.Status == "Rejected").ToList();
                    draftSurveys = new(); // No drafts for pure supervisors
                }
            }
            else
            {
                // For regular users
                draftSurveys = filteredList.Where(s => s.Status == "Draft").ToList();
                confirmedSurveys = filteredList.Where(s => s.Status == "Confirmed").ToList();
                rejectedSurveys = filteredList.Where(s => s.Status == "Rejected").ToList();
                processedSurveys = filteredList.Where(s => 
                    s.Status == "ConfirmToApprove" || 
                    s.Status == "Confirmed" || 
                    s.Status == "Rejected").ToList();
                requestedSurveys = new(); // No requested surveys shown for regular users
            }
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