using System;
using Microsoft.AspNetCore.Mvc;
using ProjectHub.Web.Models;
using ProjectHub.Web.Services;

namespace ProjectHub.Web.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ProjectService _projectService;

        public ProjectsController(ProjectService projectService)
        {
            _projectService = projectService;
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

            return View(project);
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

            return View(project);
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
            TempData["SuccessMessage"] = "项目“" + project.Name + "”已删除。";

            return RedirectToAction(nameof(Index));
        }
    }
}
