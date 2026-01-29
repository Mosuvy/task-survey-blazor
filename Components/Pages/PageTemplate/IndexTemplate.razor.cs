using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using TaskSurvey.Infrastructure.DTOs.PositionDTOs;
using TaskSurvey.Infrastructure.DTOs.TemplateDTOs;
using TaskSurvey.Infrastructure.Models;
using TaskSurvey.Infrastructure.Services.IServices;
using TaskSurvey.StateServices;

namespace TaskSurvey.Components.Pages.IndexTemplate
{
    public partial class IndexTemplate : ComponentBase
    {
        [Inject] private ITemplateService TemplateService { get; set; } = null!;
        [Inject] private IPositionService PositionService { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private AuthState AuthState { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        private List<TemplateHeaderResponseDTO> templates = new();
        private List<TemplateHeaderResponseDTO> filteredTemplates = new();
        private List<PositionResponseDTO> positions = new();
        
        private string searchQuery = "";
        private int userPositionId = 0;
        private bool isDataLoaded = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadAuthStateFromSession();
                
                userPositionId = AuthState.CurrentUser?.PositionId ?? 0;
                
                await LoadData();
                
                isDataLoaded = true;
                StateHasChanged();
            }
        }

        private async Task LoadAuthStateFromSession()
        {
            try
            {
                var userJson = await JSRuntime.InvokeAsync<string>("sessionStorage.getItem", "UserSession");
                
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
        }

        private async Task LoadData()
        {
            Console.WriteLine("DEBUG: Position ID is " + userPositionId);
            
            templates = await TemplateService.GetTemplateByPositionId(userPositionId) ?? new();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var result = templates.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var q = searchQuery.ToLower();
                result = result.Where(t => t.TemplateName.ToLower().Contains(q) || t.Id.Contains(q));
            }

            filteredTemplates = result.ToList();
        }

        private void EditTemplate(string id) => NavigationManager.NavigateTo($"/templates/form?templateId={id}");
    }
}