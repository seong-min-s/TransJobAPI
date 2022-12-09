using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TransJobAPI.Models
{
    [Table("ExaminationHistory")]
    [Index(nameof(EmployeeId), Name = "IDX_dbo_ExaminationHistory_employee")]
    [Index(nameof(JobDefinitionId), Name = "IDX_dbo_ExaminationHistory_jobDefinition")]
    public partial class ExaminationHistory
    {
        public ExaminationHistory()
        {
            ExaminationHistoryMultipleChoices = new HashSet<ExaminationHistoryMultipleChoice>();
            JobDefinitionDepthThreeOrders = new HashSet<JobDefinitionDepthThreeOrder>();
        }

        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("type")]
        [StringLength(255)]
        public string Type { get; set; }
        [Column("jobDefinitionId")]
        public long JobDefinitionId { get; set; }
        [Column("isComplete")]
        public bool IsComplete { get; set; }
        [Column("level")]
        public long Level { get; set; }
        [Column("lastModifyDateTime")]
        public DateTime? LastModifyDateTime { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }

        [ForeignKey(nameof(JobDefinitionId))]
        [InverseProperty("ExaminationHistories")]
        public virtual JobDefinition JobDefinition { get; set; }
        [InverseProperty(nameof(ExaminationHistoryMultipleChoice.ExaminationHistory))]
        public virtual ICollection<ExaminationHistoryMultipleChoice> ExaminationHistoryMultipleChoices { get; set; }
        [InverseProperty(nameof(JobDefinitionDepthThreeOrder.ExaminationHistory))]
        public virtual ICollection<JobDefinitionDepthThreeOrder> JobDefinitionDepthThreeOrders { get; set; }
    }
}
