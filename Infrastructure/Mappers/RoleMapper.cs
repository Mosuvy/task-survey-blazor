using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.RoleDTOs;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Mappers
{
    public class RoleMapper
    {
        public static RoleResponseDTO ToRoleResponseDTO(Role role)
        {
            return new RoleResponseDTO
            {
                Id = role.Id,
                RoleName = role.RoleName,
                CreatedAt = role.CreatedAt
            };
        }
    }
}