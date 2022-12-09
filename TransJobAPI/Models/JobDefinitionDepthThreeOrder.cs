using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TransJobAPI.Models
{
    [Table("JobDefinitionDepthThreeOrder")]
    public partial class JobDefinitionDepthThreeOrder
    {
        public JobDefinitionDepthThreeOrder()
        {
            ExamAssignLevels = new HashSet<ExamAssignLevel>();
        }

        [Key]
        public long Id { get; set; }
        public long ExaminationHistoryId { get; set; }
        public long JobDefinitionId { get; set; }

        [ForeignKey(nameof(ExaminationHistoryId))]
        [InverseProperty("JobDefinitionDepthThreeOrders")]
        public virtual ExaminationHistory ExaminationHistory { get; set; }
        [ForeignKey(nameof(JobDefinitionId))]
        [InverseProperty("JobDefinitionDepthThreeOrders")]
        public virtual JobDefinition JobDefinition { get; set; }
        [InverseProperty(nameof(ExamAssignLevel.Order))]
        public virtual ICollection<ExamAssignLevel> ExamAssignLevels { get; set; }
    }
}
