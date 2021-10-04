using Assignment4.Core;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Assignment4.Entities
{
    public class TaskRepository : IDesignTimeDbContextFactory<KanbanContext>
    {
        public KanbanContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<KanbanContext>()
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("Comics");

            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>()
                .UseSqlServer(connectionString);

            return new KanbanContext(optionsBuilder.Options);
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
            var jeppe = new User { Id = 1, Name = "Jeppe", Email = "korg@itu.dk" };
            var frida = new User { Id = 2, Name = "Frida", Email = "frir@itu.dk" };
            var ahmed = new User { Id = 3, Name = "Ahmed", Email = "ahga@itu.dk" };

            //Tags
            var economyTag = new Tag { Id = 1, Name = "Economy" };
            var personalTag = new Tag { Id = 2, Name = "Personal" };
            var developmentTag = new Tag { Id = 3, Name = "Development" };
            var testTag = new Tag { Id = 4, Name = "Test" };
            var programmingTag = new Tag { Id = 5, Name = "Programming" };

            //Tasks
            var task1 = new Task { Id = 1, Title = "Get better economy in the firm", AssignedTo = jeppe, Description = "We loose a lot of money, lets fix it", State = State.New, Tags = new[] { economyTag, programmingTag } };
            var task2 = new Task { Id = 2, Title = "Work on personal issues", AssignedTo = jeppe, Description = "Jeppes mental health is not good, lets fix it", State = State.Active, Tags = new[] { personalTag } };
            var task3 = new Task { Id = 3, Title = "Development of new food app", AssignedTo = frida, Description = "Build an app that 3D print food", State = State.Resolved, Tags = new[] { economyTag, developmentTag, programmingTag } };
            var task4 = new Task { Id = 4, Title = "Test Fridas new food app", AssignedTo = ahmed, Description = "Make sure Fridas food app makes delicous food", State = State.Active, Tags = new[] { personalTag, testTag } };
            var task5 = new Task { Id = 5, Title = "Program this assignment", AssignedTo = ahmed, Description = "Make the impossible happend", State = State.Active, Tags = new[] { personalTag, programmingTag } };

            //References
            economyTag.Tasks = new[] { task1, task3 };
            personalTag.Tasks = new[] { task2, task4, task5 };
            developmentTag.Tasks = new[] { task3 };
            testTag.Tasks = new[] { task4 };
            programmingTag.Tasks = new[] { task1, task3, task5 };

            jeppe.Tasks = new[] { task1, task2 };
            frida.Tasks = new[] { task3 };
            ahmed.Tasks = new[] { task4, task5 };

            context.Tasks.AddRange(
                task1, task2, task3, task4, task5
            );
            context.SaveChanges();
        }
    }
}

