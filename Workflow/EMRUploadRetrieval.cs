using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using ZREMRUpload.Models;
using ZREMRUpload.Service;

namespace ZREMRUpload.Workflow
{

    // StepBodyAsync 是 Workflow Core 提供的非同步步驟基底類別
    public class EMRUploadRetrieval : StepBodyAsync
    {
        private readonly ILogger<EMRUploadRetrieval> _logger;
        private readonly HttpClient _httpClient;
        private readonly AdminDbContext _adminDbContext;
        private readonly SharedDataStore _sharedDataStore;
        private readonly HopeCareDbContext _db;
        private readonly ApiSettings _apiSettings;


        // 透過依賴注入 (DI) 取得我們需要的服務
        public EMRUploadRetrieval(ILogger<EMRUploadRetrieval> logger, HttpClient httpClient,
            AdminDbContext adminDbContext, SharedDataStore sharedDataStore, HopeCareDbContext hopeCareDbContext, IOptions<ApiSettings> apiSettings)
        {
            _logger = logger;
            _httpClient = httpClient;
            _sharedDataStore = sharedDataStore;
            _adminDbContext = adminDbContext;
            _db = hopeCareDbContext;
            _apiSettings = apiSettings.Value;
        }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            _logger.LogInformation("開始執行電子病歷上傳 (Workflow Step)...");
            if (_apiSettings.TestModel)
            {
                _logger.LogInformation("測試模式開啟");
            }
            try
            {
                var uploadDate = DateTime.Today;

                // 取得當天完成上傳的透析表單
                var hcmHdInfo = _db.HcmHdLists.Where(x => x.ModifiedDate.Value.Date == uploadDate.Date && x.RcdStatus == "5").Select(x => new { x.Id, x.HdDate, x.PatientId, x.NurId, x.NurName, x.PatSource, x.CName, x.IdCard }).ToList();
                var hdIds = hcmHdInfo.Select(x => x.Id).ToList();

                var hdPatientIds = hcmHdInfo.Select(x => x.PatientId).ToList();

                var patients = _db.Patients.Where(x => hdPatientIds.Contains(x.Id)).ToList();

                var getPatientLastHdReportFiles = _db.HdReportFiles.Where(x => hdIds.Contains(x.HdId) && x.Status == "WU").ToLookup(x => x.PatientId)
                    .Select(x => x.OrderByDescending(y => y.Id).First()).ToList();

                // 加上判斷，未傳送的才需要新增
                var uploadData = getPatientLastHdReportFiles.Where(x => x.UploadTime == null);

                // 病摘取得巡房醫師
                var patientIds = patients.Select(x => x.Id).ToList();
                var orderPids = _db.HcdOrders.Where(x => hdIds.Contains(x.HdId)).Select(x => x.OrderPId).ToList();
                var summaryData = _db.HcBscOrders.Where(x => orderPids.Contains(x.Id) && x.OrderType == "2").ToList();

                // 取得使用者資料
                var users = _adminDbContext.Users.Select(x => new { x.Id, x.UserName, x.Email, x.DisplayName });

                // 取得使用者資訊
                var workers = _db.Workers.Select(x => new { x.IdCard, x.Name }).ToList();

                _logger.LogInformation($"取得待上傳{uploadData.Count()}筆");

                if (_apiSettings.TestModel)
                {
                    uploadData = uploadData.Take(1);
                }

                _logger.LogInformation($"實際處理{uploadData.Count()}筆");

                foreach (var item in uploadData)
                {
                    _logger.LogInformation($"處理 {item.CName}( hdid: {item.HdId}) 上傳電子病歷");
                    item.ErrorMessage = null;

                    var chartId = item.ChartId.PadLeft(10, '0');
                    if (_apiSettings.TestModel)
                    {
                        chartId = "000000000A";
                    }

                    var hddata = hcmHdInfo.First(x => x.Id == item.HdId);
                    // 從_sharedDataStore 取得就診資料，並組合成 EMRUploadModel
                    var reqData = _sharedDataStore.HdData.TryGetValue(chartId, out var req);
                    string reqNo = string.Empty;
                    if (req == null)
                    {
                        _logger.LogInformation($"無法從 SharedDataStore 取得 {chartId} 的就診資料，跳過上傳電子病歷");
                        continue;
                    }
                    if ((string.IsNullOrEmpty(hddata.PatSource) || hddata.PatSource?.ToUpper() == "O") && req.VisitDate == hddata.HdDate.Value.ToString("yyyyMMdd"))
                    {
                        reqNo = req.VisitNo;
                    }
                    else if (req.Type == "A" && DateTime.Parse(req.OrderStartDate).Date <= hddata.HdDate.Value && DateTime.Parse(req.OrderEndDate).Date >= hddata.HdDate.Value)
                    {
                        reqNo = req.VisitNo;
                    }
                    else
                    {
                        _logger.LogInformation($"{chartId} 就診資料的日期與透析日期不符，跳過上傳電子病歷。{JsonSerializer.Serialize(req)}");
                        continue;
                    }

                    var patient = patients.FirstOrDefault(x => x.Id == item.PatientId);
                    if (patient == null)
                    {
                        _logger.LogInformation($"無法找到 {item.PatientId} 的病患資料，跳過上傳電子病歷");
                        continue;
                    }

                    var hcmhd = hcmHdInfo.FirstOrDefault(x => x.Id == item.HdId);
                    var summary = summaryData.FirstOrDefault(x => x.PatientId == patient.Id && x.StartDate.Value.Date <= hcmhd.HdDate.Value.Date && (!x.EndDate.HasValue || x.EndDate.Value.Date >= hcmhd.HdDate.Value.Date));

                    // 簽章是由主責護理師及巡房醫師簽章，所以要傳二次
                    //var doctorId = string.Empty;
                    if (summary != null)
                    {
                        var user = users.FirstOrDefault(x => x.Id == summary.CreatedId);
                        if (user != null)
                        {
                            var worker = workers.FirstOrDefault(x => x.Name == user.DisplayName);
                            _logger.LogInformation($"取得巡房醫師 {user.UserName} 的簽章資訊");
                            var doctorId = _sharedDataStore.Users.TryGetValue(user.UserName, out var doctor) ? doctor.HisAccount : string.Empty;

                            if (!string.IsNullOrEmpty(doctorId))
                            {

                                var emr = new EMRUpload
                                {
                                    signonId = doctorId,
                                    patientId = chartId,
                                    patName = hcmhd.CName,
                                    patGovernId = hcmhd.IdCard,
                                    encid = reqNo,
                                    userGovenId = worker.IdCard,
                                    userName = user.DisplayName,
                                    plainData = item.FileBase64String,
                                    eventID = item.HdId.ToString(),
                                    updateSql = "",
                                    emrtype = _apiSettings.EmrType
                                };

                                var result = await SendData(emr, patient.ChartId, hcmhd.HdDate.Value);
                                // 將成功資料回寫
                                if (result.IsSuccess)
                                {
                                    item.UploadTime = DateTime.Now;
                                }
                                else
                                {
                                    item.ErrorMessage = result.ErrorMessage;
                                }
                                _db.HdReportFiles.Update(item);
                            }
                            else
                            {
                                _logger.LogInformation($"無法取得巡房醫師 {user.UserName} 的簽章資訊，跳過上傳電子病歷");
                            }
                        }
                    }
                    //var nurseId = string.Empty;
                    if (!string.IsNullOrWhiteSpace(hcmhd.NurId))
                    {
                        var user = users.FirstOrDefault(x => x.Id == hcmhd.NurId);
                        if (user != null)
                        {
                            var worker = workers.FirstOrDefault(x => x.Name == user.DisplayName);
                            _logger.LogInformation($"取得主責護理師 {user.UserName} 的簽章資訊");
                            var nurseId = _sharedDataStore.Users.TryGetValue(user.UserName, out var doctor) ? doctor.HisAccount : string.Empty;

                            if (_apiSettings.TestModel)
                            {
                                _logger.LogInformation($"測試模式開啟，模擬使用護理師帳號 N0275 簽章。，原始帳號為 {nurseId}");
                                nurseId = "N0275";
                            }

                            if (!string.IsNullOrEmpty(nurseId))
                            {

                                var emr = new EMRUpload
                                {
                                    signonId = nurseId,
                                    patientId = chartId,
                                    patName = _apiSettings.TestModel ? "測試人" : hcmhd.CName,
                                    patGovernId = _apiSettings.TestModel ? "Z299999992" : hcmhd.IdCard,
                                    encid = reqNo,
                                    userGovenId = _apiSettings.TestModel ? "J221936355" : worker.IdCard,
                                    userName = user.DisplayName,
                                    plainData = item.FileBase64String,
                                    eventID = _apiSettings.TestModel ? "60709" : item.HdId.ToString(),
                                    updateSql = "",
                                    emrtype = _apiSettings.EmrType
                                };

                                var result = await SendData(emr, patient.ChartId, hcmhd.HdDate.Value);
                                // 將成功資料回寫
                                if (result.IsSuccess)
                                {
                                    item.UploadTime = DateTime.Now;
                                }
                                else
                                {
                                    item.ErrorMessage = result.ErrorMessage;
                                }
                                _db.HdReportFiles.Update(item);
                            }
                            else
                            {
                                _logger.LogInformation($"無法取得主責護理師 {user.UserName} 的簽章資訊，跳過上傳電子病歷");
                            }
                        }
                    }
                }

                if (_db.ChangeTracker.HasChanges())
                {
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"HdReportFiles 寫入資料庫資料失敗: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"EMR Exception : {ex}");
            }

           return ExecutionResult.Next();
        }

        private async Task<HttpResult> SendData(EMRUpload model, string chartno, DateTime hdDate)
        {
            try
            {
                var apiUrl = _apiSettings.EmrUploadUrl;
                var Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                //_logger.LogInformation($"Content: {JsonSerializer.Serialize(model)}");

                if (!_apiSettings.UploadModel)
                {
                    _logger.LogInformation($"測試模式開啟，模擬發送病歷碼 {chartno} {hdDate:yyyy-MM-dd}(HdId: {model.eventID})電子病歷完成。");
                    return new HttpResult { IsSuccess = true, ReturnData = "Test mode - no actual API call made." };
                }
                var apiResult = await _httpClient.PostAsync(apiUrl, Content);
                var result = await GetResponseString(apiResult);

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"發送病歷碼 {chartno} {hdDate:yyyy-MM-dd}(HdId: {model.eventID})電子病歷完成。");
                    _logger.LogInformation($"API 回傳: {result.ReturnData}");

                    var responseData = JsonSerializer.Deserialize<EMRResult>(result.ReturnData);

                    if (responseData.status_code != (int)HttpStatusCode.OK)
                    {
                        return new HttpResult { IsSuccess = false, ErrorMessage = $"API 回傳狀態碼: {responseData.status_code}, 訊息: {responseData.msg}" };
                    }
                }
                else
                {
                    _logger.LogError($"發送病歷碼 {chartno} {hdDate:yyyy-MM-dd}(HdId: {model.eventID})電子病歷失敗。原因: {result.ErrorMessage}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"發送病歷碼 {chartno} {hdDate:yyyy-MM-dd}(HdId: {model.eventID})電子病歷失敗。原因: {ex}");
                return new HttpResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        private async Task<HttpResult> GetResponseString(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var responseError = await response.Content.ReadAsStringAsync();
                var errorMessage = $"Invoke {response.RequestMessage.RequestUri} Code: {response.StatusCode} Reason: {responseError ?? "Unknown, Can not parsed"}";
                _logger.LogError(errorMessage);

                return new HttpResult { IsSuccess = false, ErrorMessage = errorMessage };
            }

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(responseStream);

            try
            {
                return new HttpResult { IsSuccess = true, ReturnData = reader.ReadToEnd() };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new HttpResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public class HttpResult
        {
            public bool IsSuccess { get; set; }
            public string ReturnData { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}