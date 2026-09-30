using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZREMRUpload.Models
{
    [Table("hcdorders", Schema = "dbo")]
    public class HcdOrder
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("hdid")]
        public int HdId { get; set; }

        [Column("orderpid")]
        public int OrderPId { get; set; }

        [Column("patientid")]
        public int? PatientId { get; set; }

        [Column("hddate")]
        public DateTime? HdDate { get; set; }

        [Column("orderdt")]
        public DateTime? OrderDt { get; set; }

        [Column("ordercont")]
        public string OrderCont { get; set; }

        [Column("onflag")]
        public string OnFlag { get; set; }

        [Column("ontime")]
        public DateTime? OnTime { get; set; }

        [Column("onop")]
        [StringLength(20)]
        public string OnOp { get; set; }

        [Column("postpone")]
        [StringLength(80)]
        public string Postpone { get; set; }

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

        [Column("ordertype")]
        public string OrderType { get; set; }

        [Column("startdate")]
        public DateTime? StartDate { get; set; }

        [Column("enddate")]
        public DateTime? EndDate { get; set; }

        [Column("epoqty")]
        public decimal? EpoQty { get; set; }

        [Column("dpoqty")]
        public decimal? DpoQty { get; set; }
    }
}
