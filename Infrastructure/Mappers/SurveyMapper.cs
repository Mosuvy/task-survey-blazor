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
                UpdatedAtTemplate = entity.UpdatedAtTemplate,
                
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
                UpdatedAtTemplate = dto.UpdatedAtTemplate,
                
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

        public static DocumentSurvey ToEntityFromTemplate(TemplateHeader template, string surveyId, string requesterId, StatusType status)
        {
            return new DocumentSurvey
            {
                Id = surveyId,
                RequesterId = requesterId,
                TemplateHeaderId = template.Id,
                UpdatedAtTemplate = template.UpdatedAt,
                Status = status,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                
                SurveyItems = template.Items?.Select(ti => new DocumentSurveyItem
                {
                    DocumentSurveyId = surveyId,
                    TemplateItemId = ti.Id,
                    Question = ti.Question,
                    Type = ti.Type, 
                    OrderNo = ti.OrderNo,
                    Answer = "", 
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    
                    CheckBox = ti.ItemDetails?.Select(td => new DocumentItemDetail
                    {
                        TemplateItemDetailId = td.Id,
                        Item = td.Item,
                        IsChecked = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }).ToList() ?? new List<DocumentItemDetail>()
                }).ToList() ?? new List<DocumentSurveyItem>()
            };
        }
    }
}