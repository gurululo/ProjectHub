using System.Collections.Generic;

namespace ProjectHub.Web.Models.ViewModels
{
    public class ProjectListViewModel
    {
        public IList<ProjectListItemViewModel> Items { get; set; } = new List<ProjectListItemViewModel>();

        public string Keyword { get; set; }

        public ProjectStatus? Status { get; set; }
    }
}
