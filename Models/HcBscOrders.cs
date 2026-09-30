using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZREMRUpload.Models
{
    [Table("hcbscorders", Schema = "dbo")]
    public class HcBscOrders
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("patientid")]
        public int? PatientId { get; set; }

        [Column("orderdate")]
        public DateTime? OrderDate { get; set; }

        [Column("ordercont")]
        public string OrderCont { get; set; }

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

        [Column("ordertime")]
        [StringLength(5)]
        public string OrderTime { get; set; }

        [Column("ordertype")]
        public string OrderType { get; set; }

        [Column("startdate")]
        public DateTime? StartDate { get; set; }

        [Column("enddate")]
        public DateTime? EndDate { get; set; }
    }
}
