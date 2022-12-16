using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TransJobAPI.Models;

#nullable disable

namespace TransJobAPI.Contexts
{
    public partial class InitializeTestDbContext : DbContext
    {
        public InitializeTestDbContext()
        {
        }

        public InitializeTestDbContext(DbContextOptions<InitializeTestDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<ExamAssignLevel> ExamAssignLevels { get; set; }
        public virtual DbSet<ExaminationHistory> ExaminationHistories { get; set; }
        public virtual DbSet<ExaminationHistoryMultipleChoice> ExaminationHistoryMultipleChoices { get; set; }
        public virtual DbSet<JobDefinition> JobDefinitions { get; set; }
        public virtual DbSet<JobDefinitionDepthThreeOrder> JobDefinitionDepthThreeOrders { get; set; }
        public virtual DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuestionJobDefinition> QuestionJobDefinitions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=KTL-DUTY-WEB-DB; Initial Catalog=JOBTRANS_KTL; User ID=ktl;Password=smart12@#34!;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Korean_Wansung_CI_AS");

            modelBuilder.Entity<ExamAssignLevel>(entity =>
            {
                entity.HasOne(d => d.History)
                    .WithMany(p => p.ExamAssignLevels)
                    .HasForeignKey(d => d.HistoryId)
                    .HasConstraintName("FK__ExamAssig__Histo__19AACF41");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.ExamAssignLevels)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__ExamAssig__Order__1A9EF37A");
            });

            modelBuilder.Entity<ExaminationHistory>(entity =>
            {
                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.ExaminationHistories)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JOBTRANS_KTL_dbo_ExaminationHistory_employee");

                entity.HasOne(d => d.JobDefinition)
                    .WithMany(p => p.ExaminationHistories)
                    .HasForeignKey(d => d.JobDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JOBTRANS_KTL_dbo_ExaminationHistory_jobDefinition");
            });

            modelBuilder.Entity<ExaminationHistoryMultipleChoice>(entity =>
            {
                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.ExaminationHistoryMultipleChoices)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JOBTRANS_KTL_dbo_ExaminationHistoryMultipleChoice_employee");

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
                entity.HasOne(d => d.ExaminationHistory)
                    .WithMany(p => p.JobDefinitionDepthThreeOrders)
                    .HasForeignKey(d => d.ExaminationHistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__JobDefini__Exami__15DA3E5D");

                entity.HasOne(d => d.JobDefinition)
                    .WithMany(p => p.JobDefinitionDepthThreeOrders)
                    .HasForeignKey(d => d.JobDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__JobDefini__JobDe__16CE6296");
            });

            modelBuilder.Entity<MultipleChoiceQuestion>(entity =>
            {
                entity.HasOne(d => d.Question)
                    .WithMany(p => p.MultipleChoiceQuestions)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JOBTRANS_KTL_dbo_MultipleChoiceQuestion_question");
            });

            modelBuilder.Entity<QuestionJobDefinition>(entity =>
            {
                entity.HasOne(d => d.JobDefinition)
                    .WithMany(p => p.QuestionJobDefinitions)
                    .HasForeignKey(d => d.JobDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JOBTRANS_KTL_dbo_QuestionJobDefinition_jobDefinition");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionJobDefinitions)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JOBTRANS_KTL_dbo_QuestionJobDefinition_question");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
