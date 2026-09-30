using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using ZREMRUpload.Models;
using ZREMRUpload.Service;

namespace ZREMRUpload.Workflow
{
    public class UserRetrieval : StepBodyAsync
    {
        private readonly ILogger<UserRetrieval> _logger;
        private readonly HttpClient _httpClient;
        private readonly SharedDataStore _store;
        private readonly LoginService _loginService;
        private readonly ApiSettings _apiSettings;

        // 注入 SharedDataStore
        public UserRetrieval(ILogger<UserRetrieval> logger, HttpClient httpClient, SharedDataStore store, LoginService loginService, IOptions<ApiSettings> apiSettings)
        {
            _logger = logger;
            _httpClient = httpClient;
            _store = store;
            _loginService = loginService;
            _apiSettings = apiSettings.Value;
        }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            _logger.LogInformation($"【{nameof(UserRetrieval)}】開始抓取使用者帳號比對資料...");
            try
            {
                // 1. 取得最新且有效的 Token
                string token = await _loginService.GetTokenAsync();
#if !DEBUG

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError($"【{nameof(UserRetrieval)}】無法取得有效的 Token，請檢查登入服務是否正常運作。");
                    return ExecutionResult.Next();
                }
                // 2. 將 Token 設為這次 HttpClient 請求的 Bearer 授權標頭
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string apiUrl = _apiSettings.GetUsersUrl;
                var response = await _httpClient.GetFromJsonAsync<HisDataResponse<UserData>>(apiUrl);
#else
                var jsonString = System.IO.File.ReadAllText("D:\\文件區\\00 透析系統\\2023 竹榮\\user.json");
                var response = System.Text.Json.JsonSerializer.Deserialize<HisDataResponse<UserData>>(jsonString);
#endif

                if (response?.data != null)
                {
                    foreach (var user in response.data)
                    {
                        // 使用 HIS帳號 當作 Key。如果存在就更新，不存在就新增。
                        if (!string.IsNullOrEmpty(user.EmpId))
                        {
                            _store.Users.AddOrUpdate(user.EmpId, user, (key, oldValue) => user);
                        }
                    }
                    _logger.LogInformation($"【{nameof(UserRetrieval)}】成功更新 {_store.Users.Count} 筆使用者資料至 ConcurrentDictionary。內容: {string.Join("|", _store.Users)}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"【{nameof(UserRetrieval)}】發生錯誤");
            }
            return ExecutionResult.Next();
        }
    }
}
