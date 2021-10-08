using System;
using Assignment4.Core;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var task1 = new Task { Title = "Get better economy in the firm", AssignedTo = jeppe, Description = "We loose a lot of money, lets fix it", State = State.New, Tags = new[] { economyTag, programmingTag } };
            var task2 = new Task { Title = "Work on personal issues", AssignedTo = jeppe, Description = "Jeppes mental health is not good, lets fix it", State = State.Active, Tags = new[] { personalTag } };
            var task3 = new Task { Title = "Development of new food app", AssignedTo = frida, Description = "Build an app that 3D print food", State = State.Resolved, Tags = new[] { economyTag, developmentTag, programmingTag } };
            var task4 = new Task { Title = "Test Fridas new food app", AssignedTo = ahmed, Description = "Make sure Fridas food app makes delicous food", State = State.Active, Tags = new[] { personalTag, testTag } };
            var task5 = new Task { Title = "Program this assignment", AssignedTo = ahmed, Description = "Make the impossible happend", State = State.Active, Tags = new[] { personalTag, programmingTag } };

            context.Tasks.AddRange(
                task1, task2, task3, task4, task5
            );
            context.SaveChanges();
        }

        public (Response Response, int TaskId) Create(TaskCreateDTO task)
        {
            Response response = Response.Created;
            var entity = new Task{
                Title = task.Title,
                AssignedTo = _context.Users.SingleOrDefault(user => user.Id == task.AssignedToId),
                Description = task.Description,
                State = State.New,
                Tags = GetTags(task.Tags).ToList(),
            };
            if(entity.AssignedTo == null){
                return (Response.BadRequest, null);
            }   
            _context.Tasks.Add(entity);
            _context.SaveChanges();

            return(response, entity.Id);
        }

        public Response Delete(int taskId)
        {
            var entity = _context.Tasks.Find(taskId);

            if(entity == null)
            {
                return Response.NotFound;
            }
            if(entity.State == State.Resolved || entity.State == State.Closed || entity.State == State.Removed)
            {
                return Response.Conflict;
            }else{
                if(entity.State == State.Active)
                {
                    entity.State = State.Removed;
                } else{
                    _context.Tasks.Remove(entity);
                    _context.SaveChanges();
                }
                return Response.Deleted;
            }
        }

        public TaskDetailsDTO Read(int taskId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAll()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByState(State state)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllRemoved()
        {
            throw new NotImplementedException();
        }

        public Response Update(TaskUpdateDTO task)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tag> GetTags(IEnumerable<string> tags)
        {
            foreach (var tag in tags)
            {
                yield return new Tag{Name = tag};
            }
        }
    }

}

