using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TransJobAPI.Models
{
    [Table("ExaminationHistoryMultipleChoice")]
    [Index(nameof(EmployeeId), Name = "IDX_dbo_ExaminationHistoryMultipleChoice_employee")]
    [Index(nameof(ExaminationHistoryId), Name = "IDX_dbo_ExaminationHistoryMultipleChoice_examinationHistory")]
    [Index(nameof(MultipleQuestionId), Name = "IDX_dbo_ExaminationHistoryMultipleChoice_multipleChoiceQuestion")]
    public partial class ExaminationHistoryMultipleChoice
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("examinationHistoryId")]
        public long ExaminationHistoryId { get; set; }
        [Column("multipleQuestionId")]
        public long MultipleQuestionId { get; set; }
        [Column("seq")]
        public long Seq { get; set; }
        [Column("answer")]
        [StringLength(255)]
        public string Answer { get; set; }
        [Column("whether")]
        public bool? Whether { get; set; }
        [Column("writeDateTime")]
        public DateTime? WriteDateTime { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }

        [ForeignKey(nameof(ExaminationHistoryId))]
        [InverseProperty("ExaminationHistoryMultipleChoices")]
        public virtual ExaminationHistory ExaminationHistory { get; set; }
        [ForeignKey(nameof(MultipleQuestionId))]
        [InverseProperty(nameof(MultipleChoiceQuestion.ExaminationHistoryMultipleChoices))]
        public virtual MultipleChoiceQuestion MultipleQuestion { get; set; }
    }
}
