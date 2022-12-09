using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TransJobAPI.Models
{
    [Table("MultipleChoiceQuestion")]
    [Index(nameof(QuestionId), Name = "IDX_dbo_MultipleChoiceQuestion_question")]
    public partial class MultipleChoiceQuestion
    {
        public MultipleChoiceQuestion()
        {
            ExaminationHistoryMultipleChoices = new HashSet<ExaminationHistoryMultipleChoice>();
        }

        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("questionId")]
        public long QuestionId { get; set; }
        [Required]
        [Column("questionContents")]
        [StringLength(255)]
        public string QuestionContents { get; set; }
        [Required]
        [Column("answer1")]
        [StringLength(255)]
        public string Answer1 { get; set; }
        [Column("answer2")]
        [StringLength(255)]
        public string Answer2 { get; set; }
        [Column("answer3")]
        [StringLength(255)]
        public string Answer3 { get; set; }
        [Column("answer4")]
        [StringLength(255)]
        public string Answer4 { get; set; }
        [Column("answer5")]
        [StringLength(255)]
        public string Answer5 { get; set; }
        [Required]
        [Column("realAnswer")]
        [StringLength(255)]
        public string RealAnswer { get; set; }
        [Column("isDisabled")]
        public bool IsDisabled { get; set; }
        [Column("questionLevel")]
        public int? QuestionLevel { get; set; }
        [Column("jobDefinitionDepth3Name")]
        [StringLength(100)]
        public string JobDefinitionDepth3Name { get; set; }

        [ForeignKey(nameof(QuestionId))]
        [InverseProperty("MultipleChoiceQuestions")]
        public virtual Question Question { get; set; }
        [InverseProperty(nameof(ExaminationHistoryMultipleChoice.MultipleQuestion))]
        public virtual ICollection<ExaminationHistoryMultipleChoice> ExaminationHistoryMultipleChoices { get; set; }
    }
}
