using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZREMRUpload.Models
{
    public class EMRUpload
    {
        public string signonId { get; set; }
        public string patientId { get; set; }
        public string patName { get; set; }
        public string patGovernId { get; set; }
        public string encid { get; set; }
        public string userGovenId { get; set; }
        public string userName { get; set; }
        public string plainData { get; set; }
        public string eventID { get; set; }
        public string updateSql { get; set; }
        public string emrtype { get; set; }
    }
}
