using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZREMRUpload.Models
{
    public class ApiSettings
    {
        /// <summary>
        /// 登入 Url
        /// </summary>
        public string LoginUrl { get; set; }
        /// <summary>
        /// 登入使用者名稱
        /// </summary>
        public string LoginUsername { get; set; }
        /// <summary>
        /// 登入使用者密碼
        /// </summary>
        public string LoginPassword { get; set; }
        /// <summary>
        /// 取得使用者帳號資訊 Url
        /// </summary>
        public string GetUsersUrl { get; set; }
        /// <summary>
        /// 取得門診就醫序號資訊 Url
        /// </summary>
        public string GetOPDUrl { get; set; }
        /// <summary>
        /// 取得住院就醫序號資訊 Url
        /// </summary>
        public string GetIPDUrl { get; set; }
        /// <summary>
        /// 電子病歷上傳 Url
        /// </summary>
        public string EmrUploadUrl { get; set; }
        /// <summary>
        /// 是否為測試模式
        /// </summary>

        public bool TestModel { get; set; } = false;
        /// <summary>
        /// 是否要上傳電子病歷
        /// </summary>
        public bool UploadModel { get; set; } = false;

        /// <summary>
        /// 重取 Token 的 Url
        /// </summary>
        public string RefreshUrl { get; set; }

        /// <summary>
        /// 在 Token 到期前多少分鐘刷新 Token
        /// </summary>
        public int RefreshTokenBeforeExpiryMinutes { get; set; }

        /// <summary>
        /// Token 到期前的緩衝天數
        /// </summary>
        public int TokenExpiryBufferDays { get; set; }

        /// <summary>
        /// 電子病歷類型
        /// </summary>
        public string EmrType { get; set; }
    }
}
