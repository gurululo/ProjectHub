using System;
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

        public IActionResult Index()
        {
            var projects = _projectService.GetAll();
            return View(projects);
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
    }
}
