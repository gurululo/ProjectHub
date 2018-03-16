using System.Collections.Generic;

namespace ProjectHub.Web.Models.ViewModels
{
    public class TaskListViewModel
    {
        public IList<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

        public IDictionary<int, string> ProjectNames { get; set; } = new Dictionary<int, string>();
    }
}
