using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectHub.Web.Models;
using ProjectHub.Web.Models.ViewModels;
using ProjectHub.Web.Services;

namespace ProjectHub.Web.Controllers
{
    public class TasksController : Controller
    {
        private readonly TaskService _taskService;
        private readonly ProjectService _projectService;

        public TasksController(TaskService taskService, ProjectService projectService)
        {
            _taskService = taskService;
            _projectService = projectService;
        }

        public IActionResult Index()
        {
            var tasks = _taskService.GetAll();
            var projectNames = _projectService.GetAll()
                .ToDictionary(project => project.Id, project => project.Name);

            return View(new TaskListViewModel
            {
                Tasks = tasks,
                ProjectNames = projectNames
            });
        }

        public IActionResult Details(int id)
        {
            var task = _taskService.GetById(id);
            if (task == null)
            {
                return NotFound();
            }

            return View(new TaskDetailsViewModel
            {
                Task = task,
                Project = _projectService.GetById(task.ProjectId)
            });
        }

        [HttpGet]
        public IActionResult Create(int? projectId)
        {
            if (_projectService.GetAll().Count == 0)
            {
                TempData["ErrorMessage"] = "请先创建项目，再添加任务。";
                return RedirectToAction("Create", "Projects");
            }

            int? selectedProjectId = projectId;
            if (selectedProjectId.HasValue && !_projectService.Exists(selectedProjectId.Value))
            {
                selectedProjectId = null;
            }

            ViewBag.Projects = BuildProjectOptions(selectedProjectId);

            return View(new ProjectTask
            {
                ProjectId = selectedProjectId ?? 0,
                DueDate = DateTime.Today.AddDays(7),
                Status = ProjectTaskStatus.Pending
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProjectTask task)
        {
            if (task == null)
            {
                return BadRequest();
            }

            ValidateProject(task);

            if (!ModelState.IsValid)
            {
                ViewBag.Projects = BuildProjectOptions(task.ProjectId);
                return View(task);
            }

            _taskService.Create(task);
            TempData["SuccessMessage"] = "任务“" + task.Name + "”创建成功。";

            return RedirectToAction("Details", "Projects", new { id = task.ProjectId });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var task = _taskService.GetById(id);
            if (task == null)
            {
                return NotFound();
            }

            ViewBag.Projects = BuildProjectOptions(task.ProjectId);
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProjectTask task)
        {
            if (task == null)
            {
                return BadRequest();
            }

            var existing = _taskService.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            task.Id = id;
            ValidateProject(task);

            if (!ModelState.IsValid)
            {
                ViewBag.Projects = BuildProjectOptions(task.ProjectId);
                return View(task);
            }

            if (!_taskService.Update(task))
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "任务“" + task.Name + "”更新成功。";
            return RedirectToAction(nameof(Details), new { id = task.Id });
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var task = _taskService.GetById(id);
            if (task == null)
            {
                return NotFound();
            }

            return View(new TaskDetailsViewModel
            {
                Task = task,
                Project = _projectService.GetById(task.ProjectId)
            });
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var task = _taskService.GetById(id);
            if (task == null)
            {
                return NotFound();
            }

            var projectId = task.ProjectId;
            _taskService.Delete(id);
            TempData["SuccessMessage"] = "任务“" + task.Name + "”已删除。";

            if (_projectService.Exists(projectId))
            {
                return RedirectToAction("Details", "Projects", new { id = projectId });
            }

            return RedirectToAction(nameof(Index));
        }

        private void ValidateProject(ProjectTask task)
        {
            if (!_projectService.Exists(task.ProjectId))
            {
                ModelState.AddModelError(nameof(ProjectTask.ProjectId), "请选择有效的所属项目。");
            }
        }

        private IList<SelectListItem> BuildProjectOptions(int? selectedProjectId)
        {
            return _projectService.GetAll()
                .Select(project => new SelectListItem
                {
                    Value = project.Id.ToString(),
                    Text = project.Name,
                    Selected = selectedProjectId.HasValue && selectedProjectId.Value == project.Id
                })
                .ToList();
        }
    }
}
