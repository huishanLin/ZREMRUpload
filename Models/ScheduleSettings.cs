using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZREMRUpload.Models
{
    public class ScheduleSettings
    {
        public int UserRetrieval { get; set; }
        public int HdDataRetrieval { get; set; }
        public int EMRUploadRetrieval { get; set; }
    }
}
