using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assignment4.Core;

namespace Assignment4.Entities.Tests
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly KanbanContext _context;
        private readonly TaskRepository _repo;

        public TaskRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>();
            builder.UseSqlite(connection);
            var context = new KanbanContext(builder.Options);
            context.Database.EnsureCreated();

            _context = context;
            _repo = new TaskRepository(_context);
        }

        [Fact]
        public void Create_task_returns_created_and_id()
        {
            var task = new TaskCreateDTO
            {
                Title = "Test"
            };
            var actual = _repo.Create(task);
            var expected = (Response.Created, 1);

            Assert.Equal(expected, actual);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
