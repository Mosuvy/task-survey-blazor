using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.PositionDTOs;
using TaskSurvey.Infrastructure.DTOs.RoleDTOs;
using TaskSurvey.Infrastructure.DTOs.UserDTOs;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Mappers
{
    public class UserMapper
    {
        public static UserResponseDTO ToUserResponseDto(User user)
        {
            PositionResponseDTO? posDto = null;
                if (user.Position != null)
                {
                    posDto = new PositionResponseDTO
                    {
                        Id = user.Position.Id,
                        PositionLevel = user.Position.PositionLevel,
                        CreatedAt = user.Position.CreatedAt
                    };
                }

                RoleResponseDTO? roleDto = null;
                if (user.Role != null)
                {
                    roleDto = new RoleResponseDTO
                    {
                        Id = user.Role.Id,
                        RoleName = user.Role.RoleName,
                        CreatedAt = user.Role.CreatedAt
                    };
                }

                UserResponseDTO? supervisorDto = null;
                var relation = user.SupervisorRelations?.FirstOrDefault();
                if (relation?.Supervisor != null)
                {
                    var s = relation.Supervisor;
                    supervisorDto = new UserResponseDTO
                    {
                        Id = s.Id,
                        Username = s.Username,
                        PositionName = s.PositionName,
                    };
                }

                return new UserResponseDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Position = posDto,
                    PositionName = user.PositionName,
                    Role = roleDto,
                    Supervisor = supervisorDto,
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
    }
}