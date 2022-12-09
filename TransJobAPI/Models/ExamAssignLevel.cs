using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TransJobAPI.Models
{
    [Table("ExamAssignLevel")]
    public partial class ExamAssignLevel
    {
        [Key]
        public long Id { get; set; }
        public long? HistoryId { get; set; }
        public long? OrderId { get; set; }
        public long? Level { get; set; }

        [ForeignKey(nameof(HistoryId))]
        [InverseProperty(nameof(ExaminationHistory.ExamAssignLevels))]
        public virtual ExaminationHistory History { get; set; }
        [ForeignKey(nameof(OrderId))]
        [InverseProperty(nameof(JobDefinitionDepthThreeOrder.ExamAssignLevels))]
        public virtual JobDefinitionDepthThreeOrder Order { get; set; }
    }
}
