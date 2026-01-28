using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.SurveyDTOs;
using TaskSurvey.Infrastructure.DTOs.TemplateDTOs;
using TaskSurvey.Infrastructure.DTOs.UserDTOs;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Mappers
{
    public class SurveyMapper
    {
        public static SurveyHeaderResponseDTO ToSurveyResponseDTO(DocumentSurvey entity)
        {
            return new SurveyHeaderResponseDTO
            {
                DocumentId = entity.Id,
                RequesterId = entity.RequesterId,
                Status = entity.Status.ToString(),
                TemplateHeaderId = entity.TemplateHeaderId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                
                Requester = entity.Requester != null ? new UserResponseDTO 
                { 
                    Id = entity.Requester.Id, 
                    PositionName = entity.Requester.PositionName,
                    Username = entity.Requester.Username 
                } : null,

                Header = entity.Header != null ? TemplateMapper.ToTemplateHeaderResponseDTO(entity.Header) : null,

                SurveyItems = entity.SurveyItems?.Select(i => new SurveyItemResponseDTO
                {
                    Id = i.Id,
                    DocumentSurveyId = i.DocumentSurveyId,
                    TemplateItemId = i.TemplateItemId,
                    Answer = i.Answer,
                    OrderNo = i.OrderNo,
                    Question = i.Question,
                    Type = i.Type.ToString(),
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                    
                    TemplateItem = i.TemplateItem != null ? new TemplateItemResponseDTO 
                    { 
                        Id = i.TemplateItem.Id, 
                        TemplateHeaderId = i.TemplateItem.TemplateHeaderId,
                        Question = i.TemplateItem.Question,
                        Type = i.TemplateItem.Type.ToString(),
                        OrderNo = i.TemplateItem.OrderNo
                    } : null,

                    CheckBox = i.CheckBox?.Select(d => new SurveyItemDetailResponseDTO
                    {
                        Id = d.Id,
                        DocumentItemId = d.DocumentItemId,
                        TemplateItemDetailId = d.TemplateItemDetailId,
                        Item = d.Item,
                        IsChecked = d.IsChecked,
                        CreatedAt = d.CreatedAt,
                        UpdatedAt = d.UpdatedAt
                    }).ToList() ?? new()
                }).ToList() ?? new()
            };
        }

        public static DocumentSurvey ToEntity(SurveyHeaderRequestDTO dto)
        {
            return new DocumentSurvey
            {
                Id = dto.DocumentId,
                RequesterId = dto.RequesterId,
                Status = dto.Status,
                TemplateHeaderId = dto.TemplateHeaderId,
                UpdatedAt = DateTime.Now,
                
                SurveyItems = dto.SurveyItems.Select(i => new DocumentSurveyItem
                {
                    Id = i.Id,
                    DocumentSurveyId = i.DocumentSurveyId,
                    TemplateItemId = i.TemplateItemId,
                    Answer = i.Answer,
                    OrderNo = i.OrderNo,
                    Question = i.Question,
                    Type = i.Type,
                    UpdatedAt = DateTime.Now,

                    CheckBox = i.CheckBox.Select(d => new DocumentItemDetail
                    {
                        Id = d.Id,
                        DocumentItemId = d.DocumentItemId,
                        TemplateItemDetailId = d.TemplateItemDetailId,
                        Item = d.Item,
                        IsChecked = d.IsChecked,
                        UpdatedAt = DateTime.Now
                    }).ToList()
                }).ToList()
            };
        }
    }
}