using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assignment4.Core;
using System.Linq;
using System.Collections.Generic;

namespace Assignment4.Entities.Tests
{
    public class TagRepositoryTests : IDisposable
    {
        private readonly KanbanContext _context;
        private readonly TagRepository _repo;

        public TagRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>();
            builder.UseSqlite(connection);
            var context = new KanbanContext(builder.Options);
            context.Database.EnsureCreated();

            _context = context;
            _repo = new TagRepository(_context);
        }

        [Fact]

        public void Create_tag_returns_created()
        {
            var tag = new TagCreateDTO
            {
                Name = "Test"
            };
            var actual = _repo.Create(tag);
            var expected = (Response.Created, 0);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Create_existing_tag_returns_created()
        {
            var tag = new TagCreateDTO
            {
                Name = "Test"
            };
            var tag1 = new TagCreateDTO
            {
                Name = "Test"
            };
            var tagcreated = _repo.Create(tag);
            var actual = _repo.Create(tag1);
            var expected = (Response.Conflict, 0);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Delete_tag_in_use_without_force_returns_conflict()
        {
            var taskRepo = new TaskRepository(_context);
            taskRepo.Create(new TaskCreateDTO
            {
                Title = "Test",
                Tags = new List<string>() { "programmingTag" }
            });

            var acutal = _repo.Delete(0, false);
            var expected = (Response.Conflict);

            Assert.Equal(expected, acutal);
        }

        [Fact]
        public void Delete_tag_in_use_with_force_returns_deleted()
        {
            var taskRepo = new TaskRepository(_context);
            taskRepo.Create(new TaskCreateDTO
            {
                Title = "Test",
                Tags = new List<string>() { "programmingTag" }
            });

            var acutal = _repo.Delete(0, true);
            var expected = (Response.Deleted);

            Assert.Equal(expected, acutal);
            Assert.Null(_context.Tags.Find("programmingTag"));
        }

        [Fact]
        public void Delete_tag_not_in_use_retuns_deleted()
        {
            var hej = _repo.Create(new TagCreateDTO
            {
                Name = "Test"
            });

            var actual = _repo.Delete(0, false);
            var expected = Response.Deleted;

            Assert.Equal(expected, actual);
            Assert.Null(_context.Tags.Find("programmingTag"));
        }

        [Fact]
        public void Read_returns_tag()
        {
            var tag = new TagCreateDTO
            {
                Name = "Test",
            };
            var created = _repo.Create(tag);
            var read = _repo.Read(0);

            Assert.Equal(0, read.Id);
            Assert.Equal("Test", read.Name);
        }

        [Fact]
        public void Read_all_returns_all_tags()
        {
            _repo.Create(new TagCreateDTO { Name = "Tag1" });
            _repo.Create(new TagCreateDTO { Name = "Tag2" });
            _repo.Create(new TagCreateDTO { Name = "Tag3" });

            var tags = _repo.ReadAll();

            Assert.Collection(tags,
                t => Assert.Equal(new TagDTO(0, "Tag1"), t),
                t => Assert.Equal(new TagDTO(0, "Tag2"), t),
                t => Assert.Equal(new TagDTO(0, "Tag3"), t)
            );
        }

        [Fact]
        public void Update_non_existing_tag_returns_notfound()
        {
            var actual = _repo.Update(new TagUpdateDTO { Id = 1, Name = "Test" });
            var expected = Response.NotFound;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Update_existing_tag_returns_updated()
        {
            var tag = _repo.Create(new TagCreateDTO { Name = "Test" });

            var actual = _repo.Update(new TagUpdateDTO { Id = 5, Name = "Test" });
            var expected = Response.Updated;

            Assert.Equal(expected, actual);
            Assert.Equal(5, _context.Tags.Find("Test").Id);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

