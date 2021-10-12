using System;
using Assignment4.Core;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository, IDisposable
    {
        KanbanContext _context;

        public TaskRepository(KanbanContext context)
        {
            _context = context;
        }
        public static void Seed(KanbanContext context)
        {
            context.Database.ExecuteSqlRaw("DELETE dbo.Tags");
            context.Database.ExecuteSqlRaw("DELETE dbo.Tasks");
            context.Database.ExecuteSqlRaw("DELETE dbo.Users");
            context.Database.ExecuteSqlRaw("DELETE dbo.TagTask");
            // context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Tags', RESEED, 0)");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Tasks', RESEED, 0)");
            // context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Users', RESEED, 0)");
            // context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.TagTask', RESEED, 0)");


            //Users
            var jeppe = new User { Name = "Jeppe", Email = "korg@itu.dk" };
            var frida = new User { Name = "Frida", Email = "frir@itu.dk" };
            var ahmed = new User { Name = "Ahmed", Email = "ahga@itu.dk" };

            //Tags
            var economyTag = new Tag { Name = "Economy" };
            var personalTag = new Tag { Name = "Personal" };
            var developmentTag = new Tag { Name = "Development" };
            var testTag = new Tag { Name = "Test" };
            var programmingTag = new Tag { Name = "Programming" };

            //Tasks
            var task1 = new Task { Title = "Get better economy in the firm", Created = DateTime.UtcNow, StatusUpdated = DateTime.UtcNow, AssignedTo = jeppe, Description = "We loose a lot of money, lets fix it", State = State.New, Tags = new[] { economyTag, programmingTag } };
            var task2 = new Task { Title = "Work on personal issues", Created = DateTime.UtcNow, StatusUpdated = DateTime.UtcNow, AssignedTo = jeppe, Description = "Jeppes mental health is not good, lets fix it", State = State.Active, Tags = new[] { personalTag } };
            var task3 = new Task { Title = "Development of new food app", Created = DateTime.UtcNow, StatusUpdated = DateTime.UtcNow, AssignedTo = frida, Description = "Build an app that 3D print food", State = State.Resolved, Tags = new[] { economyTag, developmentTag, programmingTag } };
            var task4 = new Task { Title = "Test Fridas new food app", Created = DateTime.UtcNow, StatusUpdated = DateTime.UtcNow, AssignedTo = ahmed, Description = "Make sure Fridas food app makes delicous food", State = State.Active, Tags = new[] { personalTag, testTag } };
            var task5 = new Task { Title = "Program this assignment", Created = DateTime.UtcNow, StatusUpdated = DateTime.UtcNow, AssignedTo = ahmed, Description = "Make the impossible happend", State = State.Active, Tags = new[] { personalTag, programmingTag } };

            context.Tasks.AddRange(
                task1, task2, task3, task4, task5
            );
            context.SaveChanges();
        }

        public (Response Response, int TaskId) Create(TaskCreateDTO task)
        {
            var entity = new Task
            {
                Title = task.Title,
                AssignedTo = _context.Users.SingleOrDefault(user => user.Id == task.AssignedToId),
                Description = task.Description,
                State = State.New,
                Created = DateTime.UtcNow,
                StatusUpdated = DateTime.UtcNow
            };
            if (task.Tags != null)
            {
                entity.Tags = GetTags(task.Tags).ToList();
            }
            if (task.AssignedToId != null && entity.AssignedTo == null)
            {
                return (Response.BadRequest, 0);
            }
            _context.Tasks.Add(entity);
            _context.SaveChanges();

            return (Response.Created, entity.Id);
        }

        public Response Delete(int taskId)
        {
            var entity = _context.Tasks.Find(taskId);

            if (entity == null)
            {
                return Response.NotFound;
            }
            if (entity.State == State.Resolved || entity.State == State.Closed || entity.State == State.Removed)
            {
                return Response.Conflict;
            }
            else
            {
                if (entity.State == State.Active)
                {
                    entity.State = State.Removed;
                    _context.SaveChanges();
                }
                else
                {
                    _context.Tasks.Remove(entity);
                    _context.SaveChanges();
                }
                return Response.Deleted;
            }
        }

        public TaskDetailsDTO Read(int taskId)
        {
            return _context.Tasks.Where(task => task.Id == taskId).Select(task => new TaskDetailsDTO(
                           task.Id,
                           task.Title,
                           task.Description,
                           task.Created,
                           task.AssignedTo.Name,
                           GetTagNames(task.Tags).ToList().AsReadOnly(),
                           task.State,
                           task.StatusUpdated
            )).SingleOrDefault();
        }

        public IReadOnlyCollection<TaskDTO> ReadAll()
        {
            return _context.Tasks.Select(task => new TaskDTO(
                task.Id,
                task.Title,
                task.AssignedTo.Name,
                GetTagNames(task.Tags).ToList(),
                task.State)).ToList().AsReadOnly();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByState(State state)
        {
            return _context.Tasks.Where(task => task.State == state).Select(task => new TaskDTO(
                task.Id,
                task.Title,
                task.AssignedTo.Name,
                GetTagNames(task.Tags).ToList(),
                task.State)).ToList().AsReadOnly();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag)
        {
            return _context.Tasks.Where(task => GetTagNames(task.Tags).Contains(tag)).Select(task => new TaskDTO(
                task.Id,
                task.Title,
                task.AssignedTo.Name,
                GetTagNames(task.Tags).ToList(),
                task.State)).ToList().AsReadOnly();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId)
        {
            return _context.Tasks.Where(task => task.AssignedTo.Id == userId).Select(task => new TaskDTO(
                task.Id,
                task.Title,
                task.AssignedTo.Name,
                GetTagNames(task.Tags).ToList(),
                task.State)).ToList().AsReadOnly();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllRemoved()
        {
            return _context.Tasks.Where(task => task.State == State.Removed).Select(task => new TaskDTO(
                task.Id,
                task.Title,
                task.AssignedTo.Name,
                GetTagNames(task.Tags).ToList(),
                task.State)).ToList().AsReadOnly();
        }

        public Response Update(TaskUpdateDTO task)
        {
            var entity = _context.Tasks.Find(task.Id);
            if (entity == null)
            {
                return Response.NotFound;
            }
            if (task.AssignedToId != null && _context.Users.Where(user => user.Id == task.AssignedToId).SingleOrDefault() == null)
            {
                return Response.BadRequest;
            }
            if (task.Title != null)
            {
                entity.Title = task.Title;
            }
            entity.AssignedTo = _context.Users.Where(user => user.Id == task.AssignedToId).SingleOrDefault();
            entity.Description = task.Description;
            entity.State = task.State;

            if (task.Tags != null)
            {
                entity.Tags = GetTags(task.Tags).ToHashSet().ToList();
            }
            entity.StatusUpdated = DateTime.UtcNow;
            _context.SaveChanges();
            return Response.Updated;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private static IEnumerable<Tag> GetTags(IEnumerable<string> tags)
        {
            foreach (var tag in tags)
            {
                yield return new Tag { Name = tag };
            }
        }

        private static IEnumerable<string> GetTagNames(IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                yield return tag.Name;
            }
        }
    }
}



