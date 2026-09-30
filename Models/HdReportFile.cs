using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZREMRUpload.Models
{
    [Table("hdreportfile", Schema = "dbo")]
    public class HdReportFile
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("hdid")]
        public int HdId { get; set; }

        [Column("version")]
        public int Version { get; set; }

        [Column("patientid")]
        public int? PatientId { get; set; }

        [Column("chartid")]
        [StringLength(11)]
        public string ChartId { get; set; }

        [Column("cname")]
        [StringLength(20)]
        public string CName { get; set; }

        [Column("year")]
        public int Year { get; set; }

        [Column("month")]
        public int Month { get; set; }

        [Column("day")]
        public int Day { get; set; }

        [Column("docfilepath")]
        public string DocFilePath { get; set; }

        [Column("pdffilepath")]
        public string PdfFilePath { get; set; }

        [Column("iserror")]
        public bool IsError { get; set; } = false;

        [Column("errormessage")]
        public string ErrorMessage { get; set; }

        [Column("createdid")]
        [StringLength(60)]
        public string CreatedId { get; set; }

        [Column("createduser")]
        [StringLength(60)]
        public string CreatedUser { get; set; }

        [Column("createddate")]
        public DateTime? CreatedDate { get; set; }

        [Column("modifiedid")]
        [StringLength(60)]
        public string ModifiedId { get; set; }

        [Column("modifieduser")]
        [StringLength(60)]
        public string ModifiedUser { get; set; }

        [Column("modifieddate")]
        public DateTime? ModifiedDate { get; set; }

        [Column("code")]
        [StringLength(6)]
        public string Code { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("filebase64string")]
        public string FileBase64String { get; set; }

        [Column("base64createdtime")]
        public DateTime? Base64CreatedTime { get; set; }

        [Column("signatureid")]
        public string SignatureId { get; set; }

        [Column("uploadtime")]
        public DateTime? UploadTime { get; set; }

        [Column("transid")]
        public string TransId { get; set; }

        [Column("returnbase64")]
        public string ReturnBase64 { get; set; }

        [Column("returnmsg")]
        public string ReturnMsg { get; set; }

        [Column("signaturetime")]
        public DateTime? SignatureTime { get; set; }
    }
}