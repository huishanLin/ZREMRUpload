using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace ZREMRUpload.Workflow
{
    public class RetrievalWorkflow
    {
        // 工作流 1：更新使用者
        public class UserWorkflow : IWorkflow
        {
            public string Id => "UserWorkflow";
            public int Version => 1;
            public void Build(IWorkflowBuilder<object> builder) => builder.StartWith<UserRetrieval>();
        }

        // 工作流 2：更新病患與醫囑
        public class HdDataWorkflow : IWorkflow
        {
            public string Id => "HdDataWorkflow";
            public int Version => 1;
            public void Build(IWorkflowBuilder<object> builder) => builder.StartWith<HdDataRetrieval>();
        }

        // 工作流 3：上傳 EMR
        public class EmrUploadWorkflow : IWorkflow
        {
            public string Id => "EmrUploadWorkflow";
            public int Version => 1;
            public void Build(IWorkflowBuilder<object> builder) => builder.StartWith<EMRUploadRetrieval>();
        }
    }
}
