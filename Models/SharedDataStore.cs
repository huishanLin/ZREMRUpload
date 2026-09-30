using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZREMRUpload.Models
{
    public class SharedDataStore
    {
        // 存放使用者，Key 設為 "HIS帳號"
        public ConcurrentDictionary<string, UserData> Users { get; } = new();

        // 存放就診資料，Key 設為 "就診號"
        public ConcurrentDictionary<string, HdData> HdData { get; } = new();
        
    }
}
