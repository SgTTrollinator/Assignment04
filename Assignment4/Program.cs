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
            
            var task = new TaskUpdateDTO{
                Id = 1,
                State = State.Active
            };


            //
            //repo.Update(task);            
            Console.WriteLine(repo.Delete(1));
            //var hej = repo.Create(task);

            
            
        

        }
    }
}