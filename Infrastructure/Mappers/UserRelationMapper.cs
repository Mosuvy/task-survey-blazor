using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.UserDTOs;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Mappers
{
    public class UserRelationMapper
    {
        public static UserRelation ToUserRelationEntity(UserRequestWithRelationDTO reqDto)
        {
            return new UserRelation
            {
                UserId = reqDto.UserId,
                SupervisorId = reqDto.SupervisorId,
                CreatedAt = DateTime.Now
            };
        }
    }
}