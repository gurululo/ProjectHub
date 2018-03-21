using System;

namespace ProjectHub.Web.Models.ViewModels
{
    public class ProjectListItemViewModel
    {
        public Project Project { get; set; }

        public int TaskCount { get; set; }

        public int CompletedTaskCount { get; set; }

        public int Progress
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
