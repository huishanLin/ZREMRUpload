using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using ZREMRUpload.Models;

namespace ZREMRUpload.Service
{
    public  class WorkflowTriggerService : BackgroundService
    {
        private readonly ILogger<WorkflowTriggerService> _logger;
        private readonly IWorkflowHost _workflowHost;
        private readonly ScheduleSettings _apiSettings;

        // 透過依賴注入取得 Workflow 引擎的實體
        public WorkflowTriggerService(ILogger<WorkflowTriggerService> logger, IWorkflowHost workflowHost, IOptions<ScheduleSettings> apiSettings)
        {
            _logger = logger;
            _workflowHost = workflowHost;
            _apiSettings = apiSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("排程觸發器服務已啟動...");

            // 定義三個排程的時間間隔
            var intervalTask1 = TimeSpan.FromMinutes(_apiSettings.UserRetrieval);
            var intervalTask2 = TimeSpan.FromMinutes(_apiSettings.HdDataRetrieval);
            var intervalTask3 = TimeSpan.FromMinutes(_apiSettings.EMRUploadRetrieval);

            // 啟動三個獨立的非同步迴圈任務，讓它們並行運作互不干擾
            var task1 = RunPeriodicWorkflowAsync("UserWorkflow", intervalTask1, stoppingToken);
            var task2 = RunPeriodicWorkflowAsync("HdDataWorkflow", intervalTask2, stoppingToken);
            var task3 = RunPeriodicWorkflowAsync("EmrUploadWorkflow", intervalTask3, stoppingToken);

            // 等待所有任務（實際上它們會一直跑，直到程式被手動停止）
            await Task.WhenAll(task1, task2, task3);
        }

        /// <summary>
        /// 共用的定時觸發邏輯
        /// </summary>
        private async Task RunPeriodicWorkflowAsync(string workflowId, TimeSpan interval, CancellationToken stoppingToken)
        {
            // 只要程式沒有被終止，這個迴圈就會一直執行
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation($"準備觸發工作流：{workflowId}，當前時間：{DateTimeOffset.Now}");

                    // 呼叫 Workflow Core 啟動對應的工作流
                    await _workflowHost.StartWorkflow(workflowId);
                }
                catch (Exception ex)
                {
                    // 捕捉錯誤，避免單一工作流的錯誤導致整個計時器崩潰
                    _logger.LogError(ex, $"觸發工作流 {workflowId} 時發生異常。");
                }

                // 暫停指定的時間，時間到了之後再進行下一次迴圈
                await Task.Delay(interval, stoppingToken);
            }
        }
    }
}
