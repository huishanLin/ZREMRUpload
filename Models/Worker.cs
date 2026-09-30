using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZREMRUpload.Models
{
    [Table("worker", Schema = "dbo")]
    public class Worker
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Column("idcard")]
        public string IdCard { get; set; }

        [Column("sex")]
        public string Sex { get; set; }

        [Column("birthdate")]
        public DateTime? BirthDate { get; set; }

        [Column("position")]
        public string Position { get; set; }

        [Column("educate")]
        public string Educate { get; set; }

        [Column("isfull")]
        public string IsFull { get; set; }

        [Column("ishd")]
        public bool IsHd { get; set; }

        [Column("ispd")]
        public bool IsPd { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("rolename")]
        public string RoleName { get; set; }

        [Column("isapproval")]
        public bool IsApproval { get; set; }

        [Column("indate")]
        public DateTime? InDate { get; set; }

        [Column("outdate")]
        public DateTime? OutDate { get; set; }

        [Column("license1")]
        public string License1 { get; set; }

        [Column("license2")]
        public string License2 { get; set; }

        [Column("license3")]
        public string License3 { get; set; }

        [Column("license4")]
        public string License4 { get; set; }

        [Column("license5")]
        public string License5 { get; set; }

        [Column("license6")]
        public string License6 { get; set; }

        [Column("license7")]
        public string License7 { get; set; }

        [Column("ishbsag")]
        public string IsHbsag { get; set; }

        [Column("isantihcv")]
        public string IsAntiHcv { get; set; }

        [Column("ishdtraining")]
        public bool IsHdTraining { get; set; }

        [Column("ispdtraining")]
        public bool IsPdTraining { get; set; }

        [Column("temp")]
        public string Temp { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("div")]
        public string Div { get; set; }

        [Column("istrainoneyear")]
        public bool? IsTrainOneYear { get; set; }

        [Column("istraincourse")]
        public bool? IsTrainCourse { get; set; }

        [Column("createddate")]
        public DateTime? CreatedDate { get; set; }

        [Column("createduser")]
        public string CreatedUser { get; set; }

        [Column("modifieddate")]
        public DateTime? ModifiedDate { get; set; }

        [Column("modifieduser")]
        public string ModifiedUser { get; set; }

        [Column("createdid")]
        public string CreatedId { get; set; }

        [Column("modifiedid")]
        public string ModifiedId { get; set; }
    }
}
