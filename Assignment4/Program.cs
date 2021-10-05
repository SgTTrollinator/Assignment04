using System;
using Assignment4.Entities;
using Assignment4.Core;

namespace Assignment4
{
    public class Program
    {
        static void Main(string[] args)
        {
            var contextFactory = new KanbanContextFactory();
            var context = contextFactory.CreateDbContext(null);
            var repo = new TaskRepository(context);

            TaskRepository.Seed(context);

            var task = new TaskDTO
            {
                Title = "hej",
                State = State.New
            };

            repo.Create(task);


        }
    }
}