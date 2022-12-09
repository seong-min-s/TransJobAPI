using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TransJobAPI.Models
{
    [Table("Question")]
    public partial class Question
    {
        public Question()
        {
            MultipleChoiceQuestions = new HashSet<MultipleChoiceQuestion>();
            QuestionJobDefinitions = new HashSet<QuestionJobDefinition>();
        }

        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("type")]
        [StringLength(255)]
        public string Type { get; set; }
        [Column("level")]
        public long Level { get; set; }
        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; }
        [Column("file")]
        public byte[] File { get; set; }
        [Column("isDeleted")]
        public bool IsDeleted { get; set; }

        [InverseProperty(nameof(MultipleChoiceQuestion.Question))]
        public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        [InverseProperty(nameof(QuestionJobDefinition.Question))]
        public virtual ICollection<QuestionJobDefinition> QuestionJobDefinitions { get; set; }
    }
}
