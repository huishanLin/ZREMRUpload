using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using WorkflowCore.Interface;
using ZREMRUpload.Models;
using ZREMRUpload.Service;
using ZREMRUpload.Workflow;
using static ZREMRUpload.Workflow.RetrievalWorkflow;

namespace ZREMRUpload
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //CreateHostBuilder(args).Build().Run();
            // 1. 建立一個暫時的設定讀取器，用來初始化 Serilog
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 2. 根據 appsettings.json 的設定初始化 Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                Log.Information("應用程式啟動中...");
                var host = CreateHostBuilder(args).Build();
                var workflowHost = host.Services.GetService<IWorkflowHost>();

                // 註冊三個工作流
                workflowHost.RegisterWorkflow<UserWorkflow>();
                workflowHost.RegisterWorkflow<HdDataWorkflow>();
                workflowHost.RegisterWorkflow<EmrUploadWorkflow>();

                // 啟動 Workflow 引擎
                workflowHost.Start();

                host.Run();

            }
            catch (Exception ex)
            {
                // 捕捉啟動時的嚴重錯誤
                Log.Fatal(ex, "應用程式啟動失敗！");
            }
            finally
            {
                // 確保所有 Log 都有確實寫入檔案
                Log.CloseAndFlush();
            }

            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                // 設定 Workflow Host 的啟動與註冊
                .ConfigureServices((hostContext, services) =>
                {
                    // 1. 註冊基礎服務
                    services.AddHttpClient();

                    // 2. 註冊 Workflow Core 服務
                    services.AddWorkflow();

                    // 3. 註冊我們剛寫好的定時觸發器 (BackgroundService)
                    services.AddHostedService<WorkflowTriggerService>();
                    services.AddTransient<UserRetrieval>();
                    services.AddTransient<HdDataRetrieval>();
                    services.AddTransient<EMRUploadRetrieval>();
                    services.AddSingleton<SharedDataStore>();
                    services.AddSingleton<LoginService>();

                    // 1. 綁定強型別設定檔 (Options Pattern)
                    services.Configure<ApiSettings>(hostContext.Configuration.GetSection("ApiSettings"));
                    services.Configure<ScheduleSettings>(hostContext.Configuration.GetSection("ScheduleSettings"));

                    // 2. 註冊 PostgreSQL DbContext
                    services.AddDbContext<HopeCareDbContext>(options =>
                        options.UseNpgsql(hostContext.Configuration.GetConnectionString("hopecare")));

                    // 3. 註冊 MSSQL DbContext
                    services.AddDbContext<AdminDbContext>(options =>
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("admin")));

                    var serviceProvider = services.BuildServiceProvider();
                    
                });
    }
}
