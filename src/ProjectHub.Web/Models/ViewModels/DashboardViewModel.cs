using System;
using System.Collections.Generic;

namespace ProjectHub.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int ProjectCount { get; set; }

        public int TaskCount { get; set; }

        public int CompletedTaskCount { get; set; }

        public int InProgressProjectCount { get; set; }

        public IList<Project> RecentProjects { get; set; } = new List<Project>();

        public IList<ProjectTask> RecentTasks { get; set; } = new List<ProjectTask>();

        public IDictionary<int, string> ProjectNames { get; set; } = new Dictionary<int, string>();

        public int OpenTaskCount
        {
            get { return TaskCount - CompletedTaskCount; }
        }

        public int TaskCompletionRate
        {
            get
            {
                return TaskCount == 0
                    ? 0
                    : (int)Math.Round(CompletedTaskCount * 100.0 / TaskCount);
            }
        }
    }
}
