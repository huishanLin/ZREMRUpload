using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZREMRUpload.Models
{
    public class HisDataResponse<T>
    {
        public List<T> data { get; set; }
    }
}
