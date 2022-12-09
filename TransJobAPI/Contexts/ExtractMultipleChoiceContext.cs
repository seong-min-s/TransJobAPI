using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TransJobAPI.Models;

#nullable disable

namespace TransJobAPI.Contexts
{
    public partial class ExtractMultipleChoiceContext : DbContext
    {
        public ExtractMultipleChoiceContext()
        {
        }

        public ExtractMultipleChoiceContext(DbContextOptions<ExtractMultipleChoiceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ExaminationHistory> ExaminationHistories { get; set; }
        public virtual DbSet<ExaminationHistoryMultipleChoice> ExaminationHistoryMultipleChoices { get; set; }
        public virtual DbSet<JobDefinition> JobDefinitions { get; set; }
        public virtual DbSet<JobDefinitionDepthThreeOrder> JobDefinitionDepthThreeOrders { get; set; }
        public virtual DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        public virtual DbSet<QuestionJobDefinition> QuestionJobDefinitions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=118.67.151.133,1433; Initial Catalog=JOBTRANS_KTL; User ID=ktl;Password=smart12@#34!;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Korean_Wansung_CI_AS");

            modelBuilder.Entity<ExaminationHistory>(entity =>
            {
                entity.HasOne(d => d.JobDefinition)
                    .WithMany(p => p.ExaminationHistories)
                    .HasForeignKey(d => d.JobDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JOBTRANS_KTL_dbo_ExaminationHistory_jobDefinition");
            });

            modelBuilder.Entity<ExaminationHistoryMultipleChoice>(entity =>
            {
                entity.HasOne(d => d.ExaminationHistory)
                    .WithMany(p => p.ExaminationHistoryMultipleChoices)
                    .HasForeignKey(d => d.ExaminationHistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JOBTRANS_KTL_dbo_ExaminationHistoryMultipleChoice_examinationHistory");

                entity.HasOne(d => d.MultipleQuestion)
                    .WithMany(p => p.ExaminationHistoryMultipleChoices)
                    .HasForeignKey(d => d.MultipleQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JOBTRANS_KTL_dbo_ExaminationHistoryMultipleChoice_multipleChoiceQuestion");
            });

            modelBuilder.Entity<JobDefinition>(entity =>
            {
                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_JOBTRANS_KTL_dbo_JobDefinition_parent");
            });

            modelBuilder.Entity<JobDefinitionDepthThreeOrder>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);

                entity.HasOne(d => d.ExaminationHistory)
                    .WithMany(p => p.JobDefinitionDepthThreeOrders)
                    .HasForeignKey(d => d.ExaminationHistoryId)
                    .HasConstraintName("FK__JobDefini__Exami__10216507");
            });

            modelBuilder.Entity<QuestionJobDefinition>(entity =>
            {
                entity.HasOne(d => d.JobDefinition)
                    .WithMany(p => p.QuestionJobDefinitions)
                    .HasForeignKey(d => d.JobDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JOBTRANS_KTL_dbo_QuestionJobDefinition_jobDefinition");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
