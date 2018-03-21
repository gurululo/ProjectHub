using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectHub.Web.Models;
using ProjectHub.Web.Models.ViewModels;
using ProjectHub.Web.Services;

namespace ProjectHub.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProjectService _projectService;
        private readonly TaskService _taskService;

        public HomeController(ProjectService projectService, TaskService taskService)
        {
            _projectService = projectService;
            _taskService = taskService;
        }

        public IActionResult Index()
        {
            var projects = _projectService.GetAll();
            var tasks = _taskService.GetAll();

            return View(new DashboardViewModel
            {
                ProjectCount = projects.Count,
                TaskCount = tasks.Count,
                CompletedTaskCount = tasks.Count(task => task.Status == ProjectTaskStatus.Completed),
                InProgressProjectCount = projects.Count(project => project.Status == ProjectStatus.InProgress),
                RecentProjects = projects.Take(5).ToList(),
                RecentTasks = tasks.Take(5).ToList(),
                ProjectNames = projects.ToDictionary(project => project.Id, project => project.Name)
            });
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
