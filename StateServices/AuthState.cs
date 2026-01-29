using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.StateServices
{
    public class AuthState
    {
        private User? _currentUser;
        private bool _isAuthenticated = false;

        public event Action? OnAuthStateChanged;
        public User? CurrentUser => _currentUser;
        public bool IsAuthenticated => _isAuthenticated;

        public void SetAuthenticationState(User user)
        {
            _currentUser = user;
            _isAuthenticated = true;
            
            NotifyAuthStateChanged();
        }

        public void Logout()
        {
            _currentUser = null;
            _isAuthenticated = false;
            NotifyAuthStateChanged();
        }

        private void NotifyAuthStateChanged()
        {
            OnAuthStateChanged?.Invoke();
        }
    }
}