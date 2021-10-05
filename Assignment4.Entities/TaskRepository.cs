using System;
using Assignment4.Core;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace Assignment4.Entities
{
    public class TaskRepository : IDesignTimeDbContextFactory<KanbanContext>, ITaskRepository, IDisposable
    {

        KanbanContext context;

        public KanbanContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<KanbanContext>()
                .AddJsonFile("appsettings.json")
                .Build();

            //indsæt connectionstring her, her skal vi så lave vores migrations som bliver til vores database ud fra de lister vi har i KanbanContext
            var connectionString = configuration.GetConnectionString("Comics");

            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>()
                .UseSqlServer(connectionString);

            context = new KanbanContext(optionsBuilder.Options);
            return context;

        }
        public static void Seed(KanbanContext context)
        {
            context.Database.ExecuteSqlRaw("DELETE dbo.Tag");
            context.Database.ExecuteSqlRaw("DELETE dbo.Task");
            context.Database.ExecuteSqlRaw("DELETE dbo.User");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Tag', RESEED, 0)");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Task', RESEED, 0)");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.User', RESEED, 0)");


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

        public IReadOnlyCollection<TaskDTO> All()
        {
            throw new NotImplementedException();
        }
        public int Create(TaskDTO task)
        {
            var taskElement = new Task
            {
                Title = task.Title,
                State = task.State
            };
            context.Tasks.Add(taskElement);
            context.SaveChanges();
            return taskElement.Id;
        }

        public void Delete(int taskId)
        {
            var taskElement = (
                from task in context.Tasks
                where task.Id == taskId
                select task
            );
            context.Remove(taskElement);
            context.SaveChanges();
        }

        public TaskDetailsDTO FindById(int id)
        {
            var taskResult = (from task in context.Tasks
                              join user in context.Users
                              on task.AssignedTo.Id equals user.Id
                              where task.Id == id
                              select new TaskDetailsDTO
                              {
                                  Id = task.Id,
                                  Title = task.Title,
                                  Description = task.Description,
                                  AssignedToId = user.Id,
                                  AssignedToName = user.Name,
                                  AssignedToEmail = user.Email,
                                  //find ud af noget smart med tags
                                  //Tags = (IEnumerable<string>)task.Tags.SelectMany(tag => tag.Name),
                                  State = task.State,
                              }).Single();

            return taskResult;
        }

        public void Update(TaskDTO task)
        {
            var taskElement = (from t in context.Tasks
                               where t.Id == task.Id
                               select t).Single();

            var userElement = (from u in context.Users
                               where u.Id == taskElement.AssignedTo.Id
                               select u).Single();

            taskElement.Title = task.Title;
            if (userElement != null)
            {
                taskElement.AssignedTo = userElement;
            }
            taskElement.Description = task.Description;
            taskElement.State = task.State;
            //tags maybe
            context.SaveChanges();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

