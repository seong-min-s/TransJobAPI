using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TransJobAPI.Models
{
    [Table("Employee")]
    [Index(nameof(Code), Name = "IDX_dbo_Employee_code")]
    [Index(nameof(LoginId), Name = "IDX_dbo_Employee_loginId")]
    public partial class Employee
    {
        public Employee()
        {
            ExaminationHistories = new HashSet<ExaminationHistory>();
            ExaminationHistoryMultipleChoices = new HashSet<ExaminationHistoryMultipleChoice>();
        }

        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("code")]
        [StringLength(255)]
        public string Code { get; set; }
        [Required]
        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; }
        [Column("email")]
        [StringLength(255)]
        public string Email { get; set; }
        [Column("phoneNumber")]
        [StringLength(255)]
        public string PhoneNumber { get; set; }
        [Column("birthDate", TypeName = "date")]
        public DateTime? BirthDate { get; set; }
        [Column("gender")]
        [StringLength(255)]
        public string Gender { get; set; }
        [Required]
        [Column("permission")]
        [StringLength(255)]
        public string Permission { get; set; }
        [Column("position")]
        [StringLength(255)]
        public string Position { get; set; }
        [Column("department")]
        [StringLength(255)]
        public string Department { get; set; }
        [Column("enteringDate", TypeName = "date")]
        public DateTime? EnteringDate { get; set; }
        [Column("leavingDate", TypeName = "date")]
        public DateTime? LeavingDate { get; set; }
        [Column("loginId")]
        [StringLength(255)]
        public string LoginId { get; set; }
        [Column("encryptedPassword")]
        [StringLength(255)]
        public string EncryptedPassword { get; set; }
        [Column("isDeleted")]
        public bool IsDeleted { get; set; }
        [Column("isAccess")]
        public bool IsAccess { get; set; }

        [InverseProperty(nameof(ExaminationHistory.Employee))]
        public virtual ICollection<ExaminationHistory> ExaminationHistories { get; set; }
        [InverseProperty(nameof(ExaminationHistoryMultipleChoice.Employee))]
        public virtual ICollection<ExaminationHistoryMultipleChoice> ExaminationHistoryMultipleChoices { get; set; }
    }
}
