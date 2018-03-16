using System;
using System.Collections.Generic;
using System.Linq;
using ProjectHub.Web.Models;

namespace ProjectHub.Web.Services
{
    /// <summary>
    /// 任务数据的读取与维护。
    /// </summary>
    public class TaskService
    {
        private const string DataFileName = "tasks.json";

        private static readonly object WriteSync = new object();

        private readonly IJsonDataStore _dataStore;

        public TaskService(IJsonDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public IList<ProjectTask> GetAll()
        {
            return LoadTasks()
                .OrderByDescending(task => task.CreatedAt)
                .ThenByDescending(task => task.Id)
                .ToList();
        }

        public IList<ProjectTask> GetByProjectId(int projectId)
        {
            return LoadTasks()
                .Where(task => task.ProjectId == projectId)
                .OrderBy(task => task.Status == ProjectTaskStatus.Completed ? 1 : 0)
                .ThenBy(task => task.DueDate)
                .ThenBy(task => task.Id)
                .ToList();
        }

        public ProjectTask GetById(int id)
        {
            return LoadTasks().FirstOrDefault(task => task.Id == id);
        }

        public int CountAll()
        {
            return LoadTasks().Count;
        }

        public int CountCompleted()
        {
            return LoadTasks().Count(task => task.Status == ProjectTaskStatus.Completed);
        }

        public ProjectTask Create(ProjectTask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            Normalize(task);

            lock (WriteSync)
            {
                var tasks = LoadTasks();
                task.Id = tasks.Count == 0 ? 1 : tasks.Max(item => item.Id) + 1;

                var now = DateTime.Now;
                task.CreatedAt = now;
                task.UpdatedAt = now;

                tasks.Add(task);
                _dataStore.Write(DataFileName, tasks);
            }

            return task;
        }

        public bool Update(ProjectTask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            Normalize(task);

            lock (WriteSync)
            {
                var tasks = LoadTasks();
                var existing = tasks.FirstOrDefault(item => item.Id == task.Id);
                if (existing == null)
                {
                    return false;
                }

                existing.ProjectId = task.ProjectId;
                existing.Name = task.Name;
                existing.Description = task.Description;
                existing.DueDate = task.DueDate;
                existing.Status = task.Status;
                existing.UpdatedAt = DateTime.Now;

                _dataStore.Write(DataFileName, tasks);
                return true;
            }
        }

        public bool Delete(int id)
        {
            lock (WriteSync)
            {
                var tasks = LoadTasks();
                var existing = tasks.FirstOrDefault(task => task.Id == id);
                if (existing == null)
                {
                    return false;
                }

                tasks.Remove(existing);
                _dataStore.Write(DataFileName, tasks);
                return true;
            }
        }

        public int DeleteByProjectId(int projectId)
        {
            lock (WriteSync)
            {
                var tasks = LoadTasks();
                var removedCount = tasks.RemoveAll(task => task.ProjectId == projectId);
                if (removedCount > 0)
                {
                    _dataStore.Write(DataFileName, tasks);
                }

                return removedCount;
            }
        }

        private List<ProjectTask> LoadTasks()
        {
            return _dataStore.Read(DataFileName, () => new List<ProjectTask>()) ?? new List<ProjectTask>();
        }

        private static void Normalize(ProjectTask task)
        {
            task.Name = task.Name == null ? null : task.Name.Trim();
            task.Description = task.Description == null ? null : task.Description.Trim();
        }
    }
}
