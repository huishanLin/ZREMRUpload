using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class HdDataRetrieval : StepBodyAsync
    {
        private readonly ILogger<HdDataRetrieval> _logger;
        private readonly HttpClient _httpClient;
        private readonly SharedDataStore _store;
        private readonly LoginService _loginService;
        private readonly ApiSettings _apiSettings;

        public HdDataRetrieval(ILogger<HdDataRetrieval> logger, HttpClient httpClient, SharedDataStore store, LoginService loginService, IOptions<ApiSettings> apiSettings)
        {
            _logger = logger;
            _httpClient = httpClient;
            _store = store;
            _loginService = loginService;
            _apiSettings = apiSettings.Value;
        }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            _logger.LogInformation($"【{nameof(HdDataRetrieval)}】開始抓取病患的就診序號資料...");
            try
            {
#if !DEBUG
                // 1. 取得最新且有效的 Token
                string token = await _loginService.GetTokenAsync();

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError($"【{nameof(HdDataRetrieval)}】無法取得有效的 Token，請檢查登入服務是否正常運作。");
                    return ExecutionResult.Next();
                }
                // 2. 將 Token 設為這次 HttpClient 請求的 Bearer 授權標頭
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // 3. 處理門診就診資料
                string opdApiUrl = $"{_apiSettings.GetOPDUrl}?start_date={DateTime.Now:yyyyMMdd}&end_date={DateTime.Now:yyyyMMdd}";
                _logger.LogInformation($"【{nameof(HdDataRetrieval)}】正在從 OPD API 取得資料，URL: {opdApiUrl}");
                var opdResponse = await _httpClient.GetAsync(opdApiUrl);

                if (!opdResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"【{nameof(HdDataRetrieval)}】無法從 OPD API 取得資料，狀態碼: {opdResponse.StatusCode}");
                    string errorDetail = await opdResponse.Content.ReadAsStringAsync();
                    _logger.LogError($"【{nameof(HdDataRetrieval)}】無法從 OPD API 取得資料，錯誤訊息: {errorDetail}");
                    return ExecutionResult.Next();
                }

                var opdContent = await opdResponse.Content.ReadAsStringAsync();
                _logger.LogInformation($"【{nameof(HdDataRetrieval)}】成功從 OPD API 取得資料。內容: {opdContent}");
                var opdData = System.Text.Json.JsonSerializer.Deserialize<HisDataResponse<HdData>>(opdContent);
#else
                // 在 Debug 模式下，直接從本地 JSON 檔案讀取模擬資料
                var jsonString = System.IO.File.ReadAllText("D:\\文件區\\00 透析系統\\2023 竹榮\\opd.json");
                var opdData = System.Text.Json.JsonSerializer.Deserialize<HisDataResponse<HdData>>(jsonString);
#endif
                if (_apiSettings.TestModel)
                {
                    _logger.LogInformation($"【{nameof(HdDataRetrieval)}】目前處於測試模式，增加測試病人的就醫號。");
                    _store.HdData.AddOrUpdate("000000000A", new HdData
                    {
                        ChartNo = "000000000A",
                        VisitNo = "90951077",
                        VisitDate = DateTime.Now.ToString("yyyyMMdd")
                    }, (key, old) => old);
                }

                if (opdData?.data != null)
                {
                    foreach (var visit in opdData.data)
                    {
                        if (!string.IsNullOrEmpty(visit.ChartNo))
                        {
                            _store.HdData.AddOrUpdate(visit.ChartNo, visit, (key, old) => visit);
                        }
                    }
                }

#if !DEBUG
                // 2. 處理住院就診資料
                string ipdApiUrl = _apiSettings.GetIPDUrl;
                var ipdResponse = await _httpClient.GetAsync(ipdApiUrl);

                if (!ipdResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"【{nameof(HdDataRetrieval)}】無法從 IPD API 取得資料，狀態碼: {ipdResponse.StatusCode}");
                    string errorDetail = await ipdResponse.Content.ReadAsStringAsync();
                    _logger.LogError($"【{nameof(HdDataRetrieval)}】無法從 IPD API 取得資料，錯誤訊息: {errorDetail}");
                    return ExecutionResult.Next();
                }

                var ipdContent = await ipdResponse.Content.ReadAsStringAsync();
                _logger.LogInformation($"【{nameof(HdDataRetrieval)}】成功從 IPD API 取得資料。內容: {ipdContent}");
                var ipdData = System.Text.Json.JsonSerializer.Deserialize<HisDataResponse<HdData>>(ipdContent);
#else
                jsonString = System.IO.File.ReadAllText("D:\\文件區\\00 透析系統\\2023 竹榮\\ipd.json");
                var ipdData = System.Text.Json.JsonSerializer.Deserialize<HisDataResponse<HdData>>(jsonString);
#endif
                if (ipdData?.data != null)
                {
                    foreach (var ipd in ipdData.data)
                    {
                        // 判斷住院的醫囑起訖日期是否包含目前日期，如果不包含，則不加入資料庫
                        if (!string.IsNullOrEmpty(ipd.ChartNo) && DateTime.Parse(ipd.OrderStartDate).Date <= DateTime.Today && DateTime.Parse(ipd.OrderEndDate).Date >= DateTime.Today)
                        {
                            // 如果這個就診號已經有醫囑清單，就加進去；如果沒有，就建立一個新的 List
                            _store.HdData.AddOrUpdate(ipd.ChartNo, ipd, (key, old) => ipd);
                        }
                    }
                }

                _logger.LogInformation($"【{nameof(HdDataRetrieval)}】資料更新完成！目前就診數:{_store.HdData.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"【{nameof(HdDataRetrieval)}】發生錯誤");
            }
            return ExecutionResult.Next();
        }
    }
}
