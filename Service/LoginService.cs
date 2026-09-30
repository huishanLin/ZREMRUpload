using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ZREMRUpload.Models;

namespace ZREMRUpload.Service
{
    public class LoginService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LoginService> _logger;
        private readonly ApiSettings _apiSettings;
        private readonly string _filePath;

        private string _accessToken = string.Empty;
        private string _refreshToken = string.Empty;
        
        private DateTime _accessExpiration = DateTime.MinValue; // 15 分鐘小限
        private DateTime _sessionExpiration = DateTime.MinValue; // 7 天大限 (從第一次登入算起)

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public LoginService(ILogger<LoginService> logger, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _apiSettings = apiSettings.Value;
            // 設定 JSON 檔案儲存路徑
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "token_store.json");

            // 初始化時讀取檔案
            LoadTokenFromFile();
        }

        public async Task<string> GetTokenAsync()
        {
            // 檢查 Access Token 是否還在 15 分鐘內
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.Now.AddMinutes(1) < _accessExpiration)
            {
                return _accessToken;
            }

            await _semaphore.WaitAsync();
            try
            {
                // 雙重檢查
                if (!string.IsNullOrEmpty(_accessToken) && DateTime.Now.AddMinutes(1) < _accessExpiration)
                {
                    return _accessToken;
                }

                // 核心邏輯：判斷是否還在「第一次登入後」的 7 天內
                if (string.IsNullOrEmpty(_refreshToken) || DateTime.Now >= _sessionExpiration)
                {
                    _logger.LogInformation("已達 7 天期限或首次登入，執行【重新登入】以重置 7 天循環...");
                    await ExecuteLoginAsync();
                }
                else
                {
                    _logger.LogInformation("15 分鐘過期但仍在 7 天內，執行【Token 刷新】...");
                    await ExecuteRefreshAsync();
                }
            }
            finally
            {
                _semaphore.Release();
            }

            return _accessToken;
        }

        private async Task ExecuteLoginAsync()
        {
            var formData = new Dictionary<string, string>
            {
                { "username", _apiSettings.LoginUsername },
                { "password", _apiSettings.LoginPassword }
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _httpClient.PostAsync(_apiSettings.LoginUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result != null)
                {
                    // 關鍵點：只有這裡會重設 7 天大限
                    _sessionExpiration = DateTime.Now.AddDays(_apiSettings.TokenExpiryBufferDays);
                    UpdateTokensAndSave(result);
                    _logger.LogInformation($"登入成功。7 天大限將在 {_sessionExpiration.ToLocalTime()} 到期。");
                }
            }
            else
            {
                _logger.LogError("登入 API 呼叫失敗。");
            }
        }

        private async Task ExecuteRefreshAsync()
        {
            // 使用 QueryString 帶入 Refresh Token
            string refreshUrl = $"{_apiSettings.RefreshUrl}{Uri.EscapeDataString(_refreshToken)}";
            
            var response = await _httpClient.PostAsync(refreshUrl, null);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result != null)
                {
                    // 只更新 Token，不變動 _sessionExpiration
                    UpdateTokensAndSave(result);
                    _logger.LogInformation("刷新成功。維持原有的 7 天到期時間。");
                }
            }
            else
            {
                _logger.LogWarning("刷新失敗，可能伺服器端已過期，改採強制重新登入。");
                await ExecuteLoginAsync();
            }
        }

        private void UpdateTokensAndSave(LoginResponse response)
        {
            _accessToken = response.Token;
            _refreshToken = response.RefreshToken;
            _accessExpiration = DateTime.Now.AddMinutes(_apiSettings.RefreshTokenBeforeExpiryMinutes);

            // 包裝成物件並轉為 JSON 存檔
            var dataToSave = new TokenPersistence
            {
                AccessToken = _accessToken,
                RefreshToken = _refreshToken,
                AccessExpiration = _accessExpiration,
                SessionExpiration = _sessionExpiration
            };

            try
            {
                string jsonString = JsonSerializer.Serialize(dataToSave, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, jsonString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "寫入 Token 檔案時發生錯誤");
            }
        }

        private void LoadTokenFromFile()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    string jsonString = File.ReadAllText(_filePath);
                    var savedData = JsonSerializer.Deserialize<TokenPersistence>(jsonString);

                    if (savedData != null)
                    {
                        _accessToken = savedData.AccessToken;
                        _refreshToken = savedData.RefreshToken;
                        _accessExpiration = savedData.AccessExpiration;
                        _sessionExpiration = savedData.SessionExpiration;
                        _logger.LogInformation("已從檔案載入先前的 Token 資訊。");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "讀取 Token 檔案失敗，將採用全新登入。");
            }
        }
    }

    // 1. 定義要存成 JSON 的資料結構
    public class TokenPersistence
    {
        /// <summary>
        /// token
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// refresh_token
        /// </summary>
        public string RefreshToken { get; set; }
        /// <summary>
        /// 存取權杖 (Access Token) 的過期時間。
        /// 這是短期的效期（例如：15 分鐘），過期後需使用 Refresh Token 進行更新。
        /// </summary>
        public DateTime AccessExpiration { get; set; }

        /// <summary>
        /// 帳號工作階段 (Session) 的最終過期時間。
        /// 從第一次登入起算的長效期（例如：7 天），一旦超過此時間則必須強制重新登入。
        /// </summary>
        public DateTime SessionExpiration { get; set; }
    }
}
