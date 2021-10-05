using System;
using Assignment4.Entities;

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


        }
    }
}