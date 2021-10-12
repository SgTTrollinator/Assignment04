using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assignment4.Core;
using System.Linq;

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
        [Fact]
        public void Create_task_with_invalid_user_returns_badrequest()
        {
            var task = new TaskCreateDTO
            {
                Title = "Test",
                AssignedToId = 7000
            };

            var actual = _repo.Create(task);
            var expected = (Response.BadRequest, 0);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Delete_non_existing_task_returns_notfound()
        {
            var actual = _repo.Delete(100);
            var expected = Response.NotFound;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Delete_resolved_task_returns_conflict()
        {
            var task = new TaskCreateDTO
            {
                Title = "Test",
            };
            _repo.Create(task);
            _repo.Update(new TaskUpdateDTO
            {
                Id = 1,
                State = State.Resolved
            });

            var actual = _repo.Delete(1);
            var expected = Response.Conflict;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Delete_active_task_returns_deleted_and_updates_state()
        {
            var task = new TaskCreateDTO
            {
                Title = "Test",
            };
            var created = _repo.Create(task);
            _repo.Update(new TaskUpdateDTO
            {
                Id = 1,
                State = State.Active
            });

            var actual = _repo.Delete(1);
            var expected = Response.Deleted;

            Assert.Equal(expected, actual);
            Assert.Equal(State.Removed, _context.Tasks.Where(t => t.Id == created.TaskId).SingleOrDefault().State);
        }

        [Fact]
        public void Delete_new_task_returns_deleted_and_deletes()
        {
            var task = new TaskCreateDTO
            {
                Title = "Test",
            };
            var created = _repo.Create(task);

            var actual = _repo.Delete(1);
            var expected = Response.Deleted;

            Assert.Equal(expected, actual);
            Assert.Null(_context.Tasks.Where(t => t.Id == created.TaskId).SingleOrDefault());
        }

        [Fact]
        public void Read_returns_task()
        {
            var task = new TaskCreateDTO
            {
                Title = "Test",
            };
            var created = _repo.Create(task);
            var read = _repo.Read(1);

            Assert.Equal(1, read.Id);
            Assert.Equal("Test", read.Title);
            Assert.Null(read.Description);
            Assert.Equal(DateTime.UtcNow, read.Created, precision: TimeSpan.FromSeconds(5));
            Assert.Null(read.AssignedToName);
            Assert.Equal(State.New, read.State);
            Assert.Equal(DateTime.UtcNow, read.StateUpdated, precision: TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Read_non_existing_returns_null()
        {
            var task = _repo.Read(999);

            Assert.Null(task);
        }

        [Fact]
        public void Read_returns_all_characters()
        {
            var economyTag = new Tag { Name = "Economy" };
            var jeppe = new User { Name = "Jeppe", Email = "korg@itu.dk" };
            _context.Tasks.AddRange(
                new Task { Title = "Test1", AssignedTo = jeppe, State = State.New, Tags = new[] { economyTag } },
                new Task { Title = "Test2", AssignedTo = jeppe, State = State.Active, Tags = new[] { economyTag } },
                new Task { Title = "Test3", AssignedTo = jeppe, State = State.Resolved, Tags = new[] { economyTag } }
            );
            _context.SaveChanges();

            var characters = _repo.ReadAll();

            Assert.Collection(characters,
                c => Assert.Equal(new TaskDTO(1, "Test1", "Jeppe", characters.ElementAt(0).Tags, State.New), c),
                c => Assert.Equal(new TaskDTO(2, "Test2", "Jeppe", characters.ElementAt(1).Tags, State.Active), c),
                c => Assert.Equal(new TaskDTO(3, "Test3", "Jeppe", characters.ElementAt(2).Tags, State.Resolved), c)
            );
        }

        [Fact]
        public void Read_all_new_returns_all_new_tasks()
        {
            var economyTag = new Tag { Name = "Economy" };
            var jeppe = new User { Name = "Jeppe", Email = "korg@itu.dk" };
            _context.Tasks.AddRange(
                new Task { Title = "Test1", AssignedTo = jeppe, State = State.New, Tags = new[] { economyTag } },
                new Task { Title = "Test2", AssignedTo = jeppe, State = State.New, Tags = new[] { economyTag } },
                new Task { Title = "Test3", AssignedTo = jeppe, State = State.New, Tags = new[] { economyTag } }
            );
            _context.SaveChanges();

            var characters = _repo.ReadAllByState(State.New);

            Assert.Collection(characters,
                c => Assert.Equal(new TaskDTO(1, "Test1", "Jeppe", characters.ElementAt(0).Tags, State.New), c),
                c => Assert.Equal(new TaskDTO(2, "Test2", "Jeppe", characters.ElementAt(1).Tags, State.New), c),
                c => Assert.Equal(new TaskDTO(3, "Test3", "Jeppe", characters.ElementAt(2).Tags, State.New), c)
            );
        }

        /*[Fact]
        public void Readall_with_programmingTag_returns_all_task_with_that_tag()
        {
            var economyTag = new Tag { Name = "Economy" };
            var jeppe = new User { Name = "Jeppe", Email = "korg@itu.dk" };
            _context.Tasks.AddRange(
                new Task { Title = "Test1", AssignedTo = jeppe, State = State.New, Tags = new[] { economyTag } },
                new Task { Title = "Test2", AssignedTo = jeppe, State = State.New, Tags = new[] { economyTag } },
                new Task { Title = "Test3", AssignedTo = jeppe, State = State.New, Tags = new[] { economyTag } }
            );
            _context.SaveChanges();

            var characters = _repo.ReadAllByTag("Economy");

            Assert.Collection(characters,
                c => Assert.Equal(new TaskDTO(1, "Test1", "Jeppe", characters.ElementAt(0).Tags, State.New), c),
                c => Assert.Equal(new TaskDTO(2, "Test2", "Jeppe", characters.ElementAt(1).Tags, State.New), c),
                c => Assert.Equal(new TaskDTO(3, "Test3", "Jeppe", characters.ElementAt(2).Tags, State.New), c)
            );
        }*/

        [Fact]
        public void Readall_with_user_returns_all_task_with_that_user()
        {
            var economyTag = new Tag { Name = "Economy" };
            var jeppe = new User { Name = "Jeppe", Email = "korg@itu.dk" };
            _context.Tasks.AddRange(
                new Task { Title = "Test1", AssignedTo = jeppe, State = State.New, Tags = new[] { economyTag } },
                new Task { Title = "Test2", AssignedTo = jeppe, State = State.New, Tags = new[] { economyTag } },
                new Task { Title = "Test3", AssignedTo = jeppe, State = State.New, Tags = new[] { economyTag } }
            );
            _context.SaveChanges();

            var characters = _repo.ReadAllByUser(0);

            Assert.Collection(characters,
                c => Assert.Equal(new TaskDTO(1, "Test1", "Jeppe", characters.ElementAt(0).Tags, State.New), c),
                c => Assert.Equal(new TaskDTO(2, "Test2", "Jeppe", characters.ElementAt(1).Tags, State.New), c),
                c => Assert.Equal(new TaskDTO(3, "Test3", "Jeppe", characters.ElementAt(2).Tags, State.New), c)
            );
        }

        [Fact]
        public void Readall_removed_returns_all_removed_tasks()
        {
            var economyTag = new Tag { Name = "Economy" };
            var jeppe = new User { Name = "Jeppe", Email = "korg@itu.dk" };
            _context.Tasks.AddRange(
                new Task { Title = "Test1", AssignedTo = jeppe, State = State.Removed, Tags = new[] { economyTag } },
                new Task { Title = "Test2", AssignedTo = jeppe, State = State.Removed, Tags = new[] { economyTag } },
                new Task { Title = "Test3", AssignedTo = jeppe, State = State.Removed, Tags = new[] { economyTag } }
            );
            _context.SaveChanges();

            var characters = _repo.ReadAllRemoved();

            Assert.Collection(characters,
                c => Assert.Equal(new TaskDTO(1, "Test1", "Jeppe", characters.ElementAt(0).Tags, State.Removed), c),
                c => Assert.Equal(new TaskDTO(2, "Test2", "Jeppe", characters.ElementAt(1).Tags, State.Removed), c),
                c => Assert.Equal(new TaskDTO(3, "Test3", "Jeppe", characters.ElementAt(2).Tags, State.Removed), c)
            );
        }

        [Fact]
        public void Update_non_existing_task_returns_notfound()
        {
            var acutal = _repo.Update(new TaskUpdateDTO
            {
                Id = 9999,
                Title = "Test"
            });

            Assert.Equal(Response.NotFound, acutal);
        }

        [Fact]
        public void Update_task_with_non_existing_user_returns_badrequest()
        {
            var task = new TaskCreateDTO
            {
                Title = "Test"
            };
            _repo.Create(task);
            var acutal = _repo.Update(new TaskUpdateDTO
            {
                Id = 1,
                AssignedToId = 999
            });

            Assert.Equal(Response.BadRequest, acutal);
        }

        [Fact]
        public void Update_task_returns_updated()
        {
            var task = new TaskCreateDTO
            {
                Title = "Test"
            };
            _repo.Create(task);
            var acutal = _repo.Update(new TaskUpdateDTO
            {
                Id = 1,
                Title = "Jeg opdaterer"
            });

            Assert.Equal(Response.Updated, acutal);
            Assert.Equal("Jeg opdaterer", _context.Tasks.Where(task => task.Id == 1).SingleOrDefault().Title);
            Assert.Equal(DateTime.UtcNow, _context.Tasks.Where(task => task.Id == 1).SingleOrDefault().StatusUpdated, precision: TimeSpan.FromSeconds(5));
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
