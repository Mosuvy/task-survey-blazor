using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.UserDTOs;
using TaskSurvey.Infrastructure.Mappers;
using TaskSurvey.Infrastructure.Repositories.IRepositories;
using TaskSurvey.Infrastructure.Services.IServices;

namespace TaskSurvey.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        
        public UserService(IUserRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<UserResponseDTO>> GetUsers()
        {
            var users = await _repository.GetAllUsersAsync();
            return users.Select(UserMapper.ToUserResponseDto).ToList();
        }
        
        public async Task<UserResponseDTO?> GetUserById(string id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if(user == null) return null;
            return UserMapper.ToUserResponseDto(user);
        }

        public async Task<List<string>> GetSubordinateIds(string supervisorId)
        {
            return await _repository.GetSubordinateIdsAsync(supervisorId);
        }

        public async Task<List<UserResponseDTO>?> GetUserByRoleId(int id)
        {
            var users = await _repository.GetUserByRoleIdAsync(id);
            if(users == null) return null;
            return users.Select(UserMapper.ToUserResponseDto).ToList();
        }

        public async Task<List<UserResponseDTO>?> GetUserByPositionId(int id)
        {
            var users = await _repository.GetUserByPositionIdAsync(id);
            if(users == null) return null;
            return users.Select(UserMapper.ToUserResponseDto).ToList();
        }

        public async Task<UserResponseDTO> CreateUser(UserRequestDTO userDto)
        {
            var userEntity = UserMapper.ToUserEntity(userDto);
            var createdUser = await _repository.CreateUserAsync(userEntity, userDto.SupervisorId!);
            return UserMapper.ToUserResponseDto(createdUser);
        }

        public async Task<UserResponseDTO?> UpdateUser(string id, UserRequestDTO userDto, string? supervisor)
        {
            var userEntity = UserMapper.ToUserEntity(userDto);
            var updatedUser = await _repository.UpdateUserAsync(id, userEntity, supervisor);
            if(updatedUser == null) return null;
            return UserMapper.ToUserResponseDto(updatedUser);
        }

        public async Task<bool> DeleteUser(string id)
        {
            return await _repository.DeleteUserAsync(id);
        }
    }
}