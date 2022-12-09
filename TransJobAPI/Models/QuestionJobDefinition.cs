using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TransJobAPI.Models
{
    [Table("QuestionJobDefinition")]
    [Index(nameof(JobDefinitionId), Name = "IDX_dbo_QuestionJobDefinition_jobDefinition")]
    [Index(nameof(QuestionId), Name = "IDX_dbo_QuestionJobDefinition_question")]
    public partial class QuestionJobDefinition
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("questionId")]
        public long QuestionId { get; set; }
        [Column("jobDefinitionId")]
        public long JobDefinitionId { get; set; }

        [ForeignKey(nameof(JobDefinitionId))]
        [InverseProperty("QuestionJobDefinitions")]
        public virtual JobDefinition JobDefinition { get; set; }
        [ForeignKey(nameof(QuestionId))]
        [InverseProperty("QuestionJobDefinitions")]
        public virtual Question Question { get; set; }
    }
}
