using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ZREMRUpload.Models
{
    public class UserData
    {
        [JsonPropertyName("HIS帳號")]
        public string HisAccount { get; set; }

        [JsonPropertyName("員編")]
        public string EmpId { get; set; }
    }
}
