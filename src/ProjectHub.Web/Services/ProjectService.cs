using System;
using System.Collections.Generic;
using System.Linq;
using ProjectHub.Web.Models;

namespace ProjectHub.Web.Services
{
    /// <summary>
    /// 项目数据的读取与维护。
    /// </summary>
    public class ProjectService
    {
        private const string DataFileName = "projects.json";

        private static readonly object WriteSync = new object();

        private readonly IJsonDataStore _dataStore;

        public ProjectService(IJsonDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public IList<Project> GetAll()
        {
            return LoadProjects()
                .OrderByDescending(project => project.CreatedAt)
                .ThenByDescending(project => project.Id)
                .ToList();
        }

        public Project GetById(int id)
        {
            return LoadProjects().FirstOrDefault(project => project.Id == id);
        }

        public bool Exists(int id)
        {
            return LoadProjects().Any(project => project.Id == id);
        }

        public Project Create(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            Normalize(project);

            lock (WriteSync)
            {
                var projects = LoadProjects();
                project.Id = projects.Count == 0 ? 1 : projects.Max(item => item.Id) + 1;

                var now = DateTime.Now;
                project.CreatedAt = now;
                project.UpdatedAt = now;

                projects.Add(project);
                _dataStore.Write(DataFileName, projects);
            }

            return project;
        }

        public bool Update(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            Normalize(project);

            lock (WriteSync)
            {
                var projects = LoadProjects();
                var existing = projects.FirstOrDefault(item => item.Id == project.Id);
                if (existing == null)
                {
                    return false;
                }

                existing.Name = project.Name;
                existing.Description = project.Description;
                existing.StartDate = project.StartDate;
                existing.EndDate = project.EndDate;
                existing.Status = project.Status;
                existing.UpdatedAt = DateTime.Now;

                _dataStore.Write(DataFileName, projects);
                return true;
            }
        }

        public bool Delete(int id)
        {
            lock (WriteSync)
            {
                var projects = LoadProjects();
                var existing = projects.FirstOrDefault(project => project.Id == id);
                if (existing == null)
                {
                    return false;
                }

                projects.Remove(existing);
                _dataStore.Write(DataFileName, projects);
                return true;
            }
        }

        private List<Project> LoadProjects()
        {
            return _dataStore.Read(DataFileName, () => new List<Project>()) ?? new List<Project>();
        }

        private static void Normalize(Project project)
        {
            project.Name = project.Name == null ? null : project.Name.Trim();
            project.Description = project.Description == null ? null : project.Description.Trim();
        }
    }
}
