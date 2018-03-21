using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProjectHub.Web.Models;
using ProjectHub.Web.Models.ViewModels;
using ProjectHub.Web.Services;

namespace ProjectHub.Web.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ProjectService _projectService;
        private readonly TaskService _taskService;

        public ProjectsController(ProjectService projectService, TaskService taskService)
        {
            _projectService = projectService;
            _taskService = taskService;
        }

        public IActionResult Index(string keyword, ProjectStatus? status)
        {
            var normalizedKeyword = keyword == null ? null : keyword.Trim();

            var projects = _projectService.GetAll();
            if (!string.IsNullOrWhiteSpace(normalizedKeyword))
            {
                projects = projects
                    .Where(project => ContainsKeyword(project.Name, normalizedKeyword) ||
                                      ContainsKeyword(project.Description, normalizedKeyword))
                    .ToList();
            }

            if (status.HasValue)
            {
                projects = projects.Where(project => project.Status == status.Value).ToList();
            }

            var tasksByProject = _taskService.GetAll()
                .GroupBy(task => task.ProjectId)
                .ToDictionary(group => group.Key, group => group.ToList());

            var items = projects.Select(project =>
            {
                List<ProjectTask> projectTasks;
                if (!tasksByProject.TryGetValue(project.Id, out projectTasks))
                {
                    projectTasks = new List<ProjectTask>();
                }

                return new ProjectListItemViewModel
                {
                    Project = project,
                    TaskCount = projectTasks.Count,
                    CompletedTaskCount = projectTasks.Count(task => task.Status == ProjectTaskStatus.Completed)
                };
            }).ToList();

            return View(new ProjectListViewModel
            {
                Items = items,
                Keyword = normalizedKeyword,
                Status = status
            });
        }

        public IActionResult Details(int id)
        {
            var project = _projectService.GetById(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(new ProjectDetailsViewModel
            {
                Project = project,
                Tasks = _taskService.GetByProjectId(id)
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Project
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30),
                Status = ProjectStatus.NotStarted
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Project project)
        {
            if (project == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(project);
            }

            _projectService.Create(project);
            TempData["SuccessMessage"] = "项目“" + project.Name + "”创建成功。";

            return RedirectToAction(nameof(Details), new { id = project.Id });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var project = _projectService.GetById(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Project project)
        {
            if (project == null)
            {
                return BadRequest();
            }

            var existing = _projectService.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            project.Id = id;

            if (!ModelState.IsValid)
            {
                return View(project);
            }

            if (!_projectService.Update(project))
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "项目“" + project.Name + "”更新成功。";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var project = _projectService.GetById(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(new ProjectDetailsViewModel
            {
                Project = project,
                Tasks = _taskService.GetByProjectId(id)
            });
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var project = _projectService.GetById(id);
            if (project == null)
            {
                return NotFound();
            }

            _projectService.Delete(id);
            _taskService.DeleteByProjectId(id);

            TempData["SuccessMessage"] = "项目“" + project.Name + "”及其关联任务已删除。";

            return RedirectToAction(nameof(Index));
        }

        private static bool ContainsKeyword(string value, string keyword)
        {
            return !string.IsNullOrEmpty(value) &&
                   value.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
