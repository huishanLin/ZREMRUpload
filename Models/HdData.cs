using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ZREMRUpload.Models
{
    public class HdData
    {
        [JsonPropertyName("就診號")]
        public string VisitNo { get; set; }

        [JsonPropertyName("病歷號")]
        public string ChartNo { get; set; }

        [JsonPropertyName("就診日期")]
        public string VisitDate { get; set; }

        [JsonPropertyName("醫囑開始日期")]
        public string OrderStartDate { get; set; }

        [JsonPropertyName("醫囑截止日期")]
        public string OrderEndDate { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
