using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using TaskSurvey.Infrastructure.DTOs.PositionDTOs;
using TaskSurvey.Infrastructure.DTOs.UserDTOs;
using TaskSurvey.Infrastructure.Models;
using TaskSurvey.Infrastructure.Services.IServices;
using TaskSurvey.StateServices;

namespace TaskSurvey.Components.Pages.IndexUser
{
    public partial class IndexUser : ComponentBase
    {
        [Inject] private IUserService UserService { get; set; } = null!;
        [Inject] private IPositionService PositionService { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private AuthState AuthState { get; set; } = null!;

        private List<UserResponseDTO> users = new();
        private List<UserResponseDTO> filteredUsers = new();
        private List<PositionResponseDTO> positions = new();
        
        private string searchQuery = "";
        private int selectedRoleId = 0;
        private int selectedPosId = 0;
        private bool isSupervisor = false;
        private bool isUser = false;
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadAuthStateFromSession();
                
                isSupervisor = AuthState.CurrentUser?.RoleId == 1;
                isUser = AuthState.CurrentUser?.RoleId == 2;
                
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
            await LoadData();
        }

        private async Task LoadData()
        {
            users = await UserService.GetUsers();
            ApplyFilters();
        }

        private void OnRoleFilterChanged(ChangeEventArgs e)
        {
            selectedRoleId = int.Parse(e.Value?.ToString() ?? "0");
            ApplyFilters();
        }

        private void OnPositionFilterChanged(ChangeEventArgs e)
        {
            selectedPosId = int.Parse(e.Value?.ToString() ?? "0");
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var result = users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var q = searchQuery.ToLower();
                result = result.Where(u => u.Username.ToLower().Contains(q) || u.Id.Contains(q));
            }

            if (selectedRoleId != 0) result = result.Where(u => u.RoleId == selectedRoleId);
            if (selectedPosId != 0) result = result.Where(u => u.PositionId == selectedPosId);

            filteredUsers = result.ToList();
        }

        private void EditUser(string id) => NavigationManager.NavigateTo($"/users/form?userId={id}");
    }
}