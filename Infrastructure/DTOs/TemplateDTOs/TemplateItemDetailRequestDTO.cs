using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.DTOs.SurveyDTOs
{
    public class TemplateItemDetailRequestDTO
    {
        public int Id { get; set; }
        public required string Item { get; set; }
    }
}