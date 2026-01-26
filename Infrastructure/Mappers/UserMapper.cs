using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.PositionDTOs;
using TaskSurvey.Infrastructure.DTOs.UserDTOs;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Mappers
{
    public class UserMapper
    {
        public static UserResponseDTO ToUserResponseDto(User user)
        {
            return new UserResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                PositionId = user.PositionId,
                PositionLevel = user.Position!.PositionLevel,
                PositionName = user.PositionName,
                RoleId = user.RoleId,
                RoleName = user.Role!.RoleName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public static User ToUserEntity(UserRequestDTO req)
        {
            return new User
            {
                Username = req.Username,
                PasswordHash = req.PasswordHash,
                PositionId = req.PositionId,
                PositionName = req.PositionName,
                RoleId = req.RoleId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        public static UserRelation ToUserRelationEntity(string newUserId, string supervisorId)
        {
            return new UserRelation
            {
                UserId = newUserId,
                SupervisorId = supervisorId,
                CreatedAt = DateTime.Now
            };
        }
    }
}