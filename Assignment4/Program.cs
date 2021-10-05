using System;
using Assignment4.Entities;
using Assignment4.Core;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Assignment4
{
    public class Program
    {
        static void Main(string[] args)
        {
            var contextFactory = new KanbanContextFactory();
            var context = contextFactory.CreateDbContext(null);
            var repo = new TaskRepository(context);
            repo.Delete(6);
            
            //Console.WriteLine(repo.All().ToString());

            /*var task = new TaskDTO
            {
                Id = 6,
                Title = "god davs",
                State = State.New
            };
            
            
            
            repo.Update(task);*/
        }
    }
}