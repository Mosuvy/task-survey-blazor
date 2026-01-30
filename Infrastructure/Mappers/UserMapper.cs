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
                var supRel = user.SupervisorRelations?.FirstOrDefault();
                if (supRel?.Supervisor != null)
                {
                    supervisorDto = new UserResponseDTO
                    {
                        Id = supRel.Supervisor.Id,
                        Username = supRel.Supervisor.Username,
                        PositionName = supRel.Supervisor.PositionName,
                    };
                }

                UserResponseDTO? subordinatesDto = null;
                var subRel = user.SubordinateRelations?.FirstOrDefault();
                if (subRel?.User != null)
                {
                    subordinatesDto = new UserResponseDTO
                    {
                        Id = subRel.User.Id,
                        Username = subRel.User.Username,
                        PositionName = subRel.User.PositionName,
                    };
                }

                return new UserResponseDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    PositionId = user.PositionId,
                    Position = posDto,
                    PositionName = user.PositionName,
                    RoleId = user.RoleId,
                    Role = roleDto,
                    Supervisor = supervisorDto,
                    Subordinates = subordinatesDto,
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