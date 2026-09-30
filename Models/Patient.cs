using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZREMRUpload.Models
{
    [Table("patient", Schema = "dbo")]
    public class Patient
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("chartid")]
        public string ChartId { get; set; }

        [Column("chid")]
        public string ChId { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("birthday")]
        public DateTime Birthday { get; set; }

        [Column("sex")]
        public string Sex { get; set; }

        [Column("marry")]
        public string Marry { get; set; }

        [Column("code")]
        public string Code { get; set; }

        [Column("curedoc")]
        public string CureDoc { get; set; }

        [Column("county0")]
        public string County0 { get; set; }

        [Column("county")]
        public string County { get; set; }

        [Column("realm")]
        public string Realm { get; set; }

        [Column("mailno")]
        public string MailNo { get; set; }

        [Column("village")]
        public string Village { get; set; }

        [Column("street")]
        public string Street { get; set; }

        [Column("educate")]
        public string Educate { get; set; }

        [Column("vocation")]
        public string Vocation { get; set; }

        [Column("father")]
        public string Father { get; set; }

        [Column("relation")]
        public string Relation { get; set; }

        [Column("ab")]
        public string Ab { get; set; }

        [Column("heartcard")]
        public string HeartCard { get; set; }

        [Column("notheartcard")]
        public string NotHeartCard { get; set; }

        [Column("isnative")]
        public bool IsNative { get; set; }

        [Column("iswelfare")]
        public bool IsWelfare { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("status1")]
        public string Status1 { get; set; }

        [Column("date")]
        public DateTime? Date { get; set; }

        [Column("firstdate")]
        public DateTime? FirstDate { get; set; }

        [Column("begindate")]
        public DateTime BeginDate { get; set; }

        [Column("dropdate")]
        public DateTime? DropDate { get; set; }

        [Column("diedate")]
        public DateTime? DieDate { get; set; }

        [Column("idiopa")]
        public string Idiopa { get; set; }

        [Column("idiopathy")]
        public string Idiopathy { get; set; }

        [Column("isptx")]
        public bool IsPtx { get; set; }

        [Column("ptxdate")]
        public DateTime? PtxDate { get; set; }

        [Column("hcvdate")]
        public DateTime? HcvDate { get; set; }

        [Column("imagedata")]
        public byte[] ImageData { get; set; }

        [Column("imagemimetype")]
        public string ImageMimeType { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("streetno")]
        public string StreetNo { get; set; }

        [Column("telno")]
        public string TelNo { get; set; }

        [Column("mobileno")]
        public string MobileNo { get; set; }

        [Column("idcard")]
        public string IdCard { get; set; }

        [Column("nephhosp")]
        public string NephHosp { get; set; }

        [Column("nephstartdate")]
        public DateTime? NephStartDate { get; set; }

        [Column("nephlastdate")]
        public DateTime? NephLastDate { get; set; }

        [Column("isforeign")]
        public bool? IsForeign { get; set; }

        [Column("peacedate")]
        public DateTime? PeaceDate { get; set; }

        [Column("createddate")]
        public DateTime? CreatedDate { get; set; }

        [Column("createduser")]
        public string CreatedUser { get; set; }

        [Column("modifieddate")]
        public DateTime? ModifiedDate { get; set; }

        [Column("modifieduser")]
        public string ModifiedUser { get; set; }

        [Column("heartstartcarddate")]
        public DateTime? HeartStartCardDate { get; set; }

        [Column("heartendcarddate")]
        public DateTime? HeartEndCardDate { get; set; }

        [Column("notheartstartcarddate")]
        public DateTime? NotHeartStartCardDate { get; set; }

        [Column("notheartendcarddate")]
        public DateTime? NotHeartEndCardDate { get; set; }

        [Column("createdid")]
        public string CreatedId { get; set; }

        [Column("modifiedid")]
        public string ModifiedId { get; set; }

        [Column("allergyflag")]
        public string AllergyFlag { get; set; }

        [Column("allergydesc")]
        [StringLength(200)]
        public string AllergyDesc { get; set; }

        [Column("allergydrug")]
        [StringLength(200)]
        public string AllergyDrug { get; set; }

        [Column("rhesus")]
        public string Rhesus { get; set; }

        [Column("hbsag")]
        public string HbsAg { get; set; }

        [Column("hbs")]
        public string Hbs { get; set; }

        [Column("hcv")]
        public string Hcv { get; set; }

        [Column("remark")]
        [StringLength(2000)]
        public string Remark { get; set; }

        [Column("remarkspec")]
        [StringLength(500)]
        public string RemarkSpec { get; set; }

        [Column("nfcno")]
        [StringLength(50)]
        public string NfcNo { get; set; }

        [Column("dnrflag")]
        [StringLength(1)]
        public string DnrFlag { get; set; }

        [Column("dnrcontent")]
        [StringLength(86)]
        public string DnrContent { get; set; }

        [Column("repdoc")]
        [StringLength(10)]
        public string RepDoc { get; set; }

        [Column("tags")]
        [StringLength(200)]
        public string Tags { get; set; }
    }
}