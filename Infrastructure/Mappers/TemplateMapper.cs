using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.PositionDTOs;
using TaskSurvey.Infrastructure.DTOs.TemplateDTOs;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Mappers
{
    public class TemplateMapper
    {
        public static TemplateHeaderResponseDTO ToTemplateHeaderResponseDTO(TemplateHeader entity)
        {
            return new TemplateHeaderResponseDTO
            {
                Id = entity.Id,
                TemplateName = entity.TemplateName,
                PositionId = entity.PositionId,
                Theme = entity.Theme,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Position = entity.Position != null ? new PositionResponseDTO 
                { 
                    Id = entity.Position.Id, 
                    PositionLevel = entity.Position.PositionLevel 
                } : null,
                Items = entity.Items?.OrderBy(item => item.OrderNo).Select(item => new TemplateItemResponseDTO
                {
                    Id = item.Id,
                    TemplateHeaderId = item.TemplateHeaderId,
                    Question = item.Question,
                    Type = item.Type.ToString(),
                    OrderNo = item.OrderNo,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                    ItemDetails = item.ItemDetails?.Select(detail => new TemplateItemDetailResponseDTO
                    {
                        Id = detail.Id,
                        TemplateItemId = detail.TemplateItemId,
                        Item = detail.Item,
                        CreatedAt = detail.CreatedAt,
                        UpdatedAt = detail.UpdatedAt
                    }).ToList() ?? new()
                }).ToList() ?? new()
            };
        }

        public static TemplateHeader ToEntity(TemplateHeaderRequestDTO dto)
        {
            return new TemplateHeader
            {
                Id = dto.Id ?? string.Empty,
                TemplateName = dto.TemplateName,
                PositionId = dto.PositionId,
                Theme = dto.Theme,
                UpdatedAt = DateTime.Now,
                Items = dto.Items.Select(i => new TemplateItem
                {
                    Id = i.Id,
                    OrderNo = i.OrderNo,
                    Question = i.Question,
                    Type = i.Type,
                    UpdatedAt = DateTime.Now,
                    ItemDetails = i.ItemDetails.Select(d => new TemplateItemDetail
                    {
                        Id = d.Id,
                        Item = d.Item,
                        UpdatedAt = DateTime.Now
                    }).ToList()
                }).ToList()
            };
        }
    }
}