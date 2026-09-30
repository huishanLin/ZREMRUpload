using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZREMRUpload.Models
{
    [Table("hcmhdlist", Schema = "dbo")]
    public class HcmHdList
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("hdweekid")]
        public long? HdWeekId { get; set; }

        [Column("hddate")]
        public DateTime? HdDate { get; set; }

        [Column("timesegment")]
        public int? TimeSegment { get; set; }

        [Column("section")]
        [StringLength(8)]
        public string Section { get; set; }

        [Column("bedno")]
        [StringLength(8)]
        public string BedNo { get; set; }

        [Column("patientid")]
        public int? PatientId { get; set; }

        [Column("chartid")]
        [StringLength(11)]
        public string ChartId { get; set; }

        [Column("cname")]
        [StringLength(20)]
        public string CName { get; set; }

        [Column("birthday")]
        public DateTime? Birthday { get; set; }

        [Column("sex")]
        public string Sex { get; set; }

        [Column("idcard")]
        [StringLength(15)]
        public string IdCard { get; set; }

        [Column("ab")]
        [StringLength(2)]
        public string Ab { get; set; }

        [Column("rhesus")]
        public string Rhesus { get; set; }

        [Column("docid")]
        [StringLength(60)]
        public string DocId { get; set; }

        [Column("docname")]
        [StringLength(20)]
        public string DocName { get; set; }

        [Column("visitmethod")]
        [StringLength(8)]
        public string VisitMethod { get; set; }

        [Column("patsource")]
        public string PatSource { get; set; }

        [Column("seqno")]
        [StringLength(20)]
        public string SeqNo { get; set; }

        [Column("inbedno")]
        [StringLength(6)]
        public string InBedNo { get; set; }

        [Column("altertype")]
        public string AlterType { get; set; }

        [Column("rcdstatus")]
        public string RcdStatus { get; set; }

        [Column("opdflag")]
        public string OpdFlag { get; set; } = "0";

        [Column("preparedt")]
        public DateTime? PrepareDt { get; set; }

        [Column("checkindt")]
        public DateTime? CheckInDt { get; set; }

        [Column("startdt")]
        public DateTime? StartDt { get; set; }

        [Column("enddt")]
        public DateTime? EndDt { get; set; }

        [Column("systolics")]
        public short? Systolics { get; set; }

        [Column("presuretypes")]
        public string PresureTypeS { get; set; }

        [Column("systolice")]
        public short? Systolice { get; set; }

        [Column("presuretypee")]
        public string PresureTypeE { get; set; }

        [Column("bodytemperature")]
        public decimal? BodyTemperature { get; set; }

        [Column("skinevaluate")]
        [StringLength(10)]
        public string SkinEvaluate { get; set; }

        [Column("reactionarea")]
        [StringLength(30)]
        public string ReactionArea { get; set; }

        [Column("skinreaction")]
        [StringLength(10)]
        public string SkinReaction { get; set; }

        [Column("infection")]
        [StringLength(15)]
        public string Infection { get; set; }

        [Column("clotting")]
        [StringLength(15)]
        public string Clotting { get; set; }

        [Column("bleeding")]
        public string Bleeding { get; set; }

        [Column("bleedingamt")]
        public short? BleedingAmt { get; set; }

        [Column("nurid")]
        [StringLength(60)]
        public string NurId { get; set; }

        [Column("nurname")]
        [StringLength(20)]
        public string NurName { get; set; }

        [Column("epotype")]
        public string EpoType { get; set; }

        [Column("epo1")]
        public decimal? Epo1 { get; set; }

        [Column("fe")]
        public decimal? Fe { get; set; }

        [Column("hct")]
        public decimal? Hct { get; set; }

        [Column("blooddrawpre")]
        public string BloodDrawPre { get; set; }

        [Column("blooddrawop1")]
        [StringLength(20)]
        public string BloodDrawOp1 { get; set; }

        [Column("blooddrawpost")]
        public string BloodDrawPost { get; set; }

        [Column("blooddrawop2")]
        [StringLength(20)]
        public string BloodDrawOp2 { get; set; }

        [Column("akclot")]
        public string AkClot { get; set; }

        [Column("chambera")]
        public string ChamberA { get; set; }

        [Column("chamberv")]
        public string ChamberV { get; set; }

        [Column("testitem")]
        [StringLength(50)]
        public string TestItem { get; set; }

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

        [Column("arterialcnt")]
        public short? ArterialCnt { get; set; }

        [Column("venouscnt")]
        public short? VenousCnt { get; set; }

        [Column("complication")]
        public string Complication { get; set; }

        [Column("diastolics")]
        public short? Diastolics { get; set; }

        [Column("diastolice")]
        public short? Diastolice { get; set; }

        [Column("pulses")]
        public short? Pulses { get; set; }

        [Column("pulsee")]
        public short? PulseE { get; set; }

        [Column("hctimes")]
        public decimal? HcTimes { get; set; }

        [Column("nfcno")]
        [StringLength(50)]
        public string NfcNo { get; set; }

        [Column("ondate")]
        public DateTime? OnDate { get; set; }

        [Column("offdate")]
        public DateTime? OffDate { get; set; }

        [Column("station")]
        [StringLength(8)]
        public string Station { get; set; }

        [Column("apply")]
        [StringLength(8)]
        public string Apply { get; set; }

        [Column("liqb")]
        [StringLength(10)]
        public string Liqb { get; set; }

        [Column("diastolice2")]
        public short? Diastolice2 { get; set; }

        [Column("systolice2")]
        public short? Systolice2 { get; set; }

        [Column("diastolice3")]
        public short? Diastolice3 { get; set; }

        [Column("systolice3")]
        public short? Systolice3 { get; set; }
    }
}
