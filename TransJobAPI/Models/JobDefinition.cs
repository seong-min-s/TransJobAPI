using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TransJobAPI.Models
{
    [Table("JobDefinition")]
    [Index(nameof(ParentId), Name = "IDX_dbo_JobDefinition_parent")]
    public partial class JobDefinition
    {
        public JobDefinition()
        {
            ExaminationHistories = new HashSet<ExaminationHistory>();
            InverseParent = new HashSet<JobDefinition>();
            JobDefinitionDepthThreeOrders = new HashSet<JobDefinitionDepthThreeOrder>();
            QuestionJobDefinitions = new HashSet<QuestionJobDefinition>();
        }

        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("depth")]
        public long Depth { get; set; }
        [Required]
        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; }
        [Column("parentId")]
        public long? ParentId { get; set; }
        [Required]
        [Column("displayOrder")]
        [StringLength(255)]
        public string DisplayOrder { get; set; }
        [Column("isDeleted")]
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(ParentId))]
        [InverseProperty(nameof(JobDefinition.InverseParent))]
        public virtual JobDefinition Parent { get; set; }
        [InverseProperty(nameof(ExaminationHistory.JobDefinition))]
        public virtual ICollection<ExaminationHistory> ExaminationHistories { get; set; }
        [InverseProperty(nameof(JobDefinition.Parent))]
        public virtual ICollection<JobDefinition> InverseParent { get; set; }
        [InverseProperty(nameof(JobDefinitionDepthThreeOrder.JobDefinition))]
        public virtual ICollection<JobDefinitionDepthThreeOrder> JobDefinitionDepthThreeOrders { get; set; }
        [InverseProperty(nameof(QuestionJobDefinition.JobDefinition))]
        public virtual ICollection<QuestionJobDefinition> QuestionJobDefinitions { get; set; }
    }
}
