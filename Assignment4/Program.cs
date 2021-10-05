using System;
using Assignment04.Entities;

namespace Assignment4
{
    class Program
    {
        static void Main(string[] args)
        {
            //indsæt
            var connectionString = configuration.GetConnectionString("Comics");
            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseSqlServer(connectionString);
            using var context = new KanbanContext(optionsBuilder.Options);
            //det her tilføjer alt vi har sat op i seed funktionen til databasen
            TaskRepository.Seed(context);

            //her kan vi evt. query på databasen

        }
    }
}