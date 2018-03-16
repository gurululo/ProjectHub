using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectHub.Web.Models.ViewModels
{
    public class ProjectDetailsViewModel
    {
        public Project Project { get; set; }

        public IList<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

        public int TaskCount
        {
            get { return Tasks.Count; }
        }

        public int CompletedTaskCount
        {
            get { return Tasks.Count(task => task.Status == ProjectTaskStatus.Completed); }
        }

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
