using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransJobAPI.Controllers
{
    public class ExaminationHistoryMultipleChoiceDTO
    {
        public long EmployeeId { get; set; }
        public long ExaminationHistoryId { get; set; }
        public long MultipleQuestionId { get; set; }
        public long JobDefinitionId { get; set; }
        public string Contents { get; set; }
        public string Answer { get; set; }
    }
}
