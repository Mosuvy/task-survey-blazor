using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.UserDTOs;

namespace TaskSurvey.Infrastructure.Services.IServices
{
    public interface IUserService
    {
        Task<List<UserResponseDTO>> GetUsers();
        Task<UserResponseDTO?> GetUserById(string id);
        Task<List<UserResponseDTO>?> GetUserByRoleId(int id);
        Task<List<UserResponseDTO>?> GetUserByPositionId(int id);
        Task<UserResponseDTO> CreateUser(UserRequestDTO userDto);
        Task<UserResponseDTO?> UpdateUser(string id, UserRequestDTO userDto, string? supervisor);
        Task<bool> DeleteUser(string id);
    }
}