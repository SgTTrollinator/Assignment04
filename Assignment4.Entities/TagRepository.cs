using System;
using Assignment4.Core;
using System.Collections.Generic;
using System.Linq;

namespace Assignment4.Entities
{
    public class TagRepository : ITagRepository, IDisposable
    {
        KanbanContext _context;

        public TagRepository(KanbanContext context)
        {
            _context = context;
        }
        public (Response Response, int TagId) Create(TagCreateDTO tag)
        {
            var entity = new Tag { Name = tag.Name };
            if (_context.Tags.Find(entity.Name) != null)
            {
                return (Response.Conflict, 0);
            }
            _context.Tags.Add(entity);
            _context.SaveChanges();
            return (Response.Created, entity.Id);
        }

        public Response Delete(int tagId, bool force = false)
        {
            var entity = _context.Tags.Where(tag => tag.Id == tagId).SingleOrDefault();
            if (entity.Tasks.Count() > 0 && !force)
            {
                return Response.Conflict;
            }
            _context.Tags.Remove(entity);
            _context.SaveChanges();
            return Response.Deleted;
        }

        public TagDTO Read(int tagId)
        {
            return _context.Tags.Where(tag => tag.Id == tagId).Select(tag => new TagDTO(
                           tag.Id,
                           tag.Name
            )).SingleOrDefault();
        }

        public IReadOnlyCollection<TagDTO> ReadAll()
        {
            return _context.Tags.Select(tag => new TagDTO(
                tag.Id,
                tag.Name)).ToList().AsReadOnly();
        }

        public Response Update(TagUpdateDTO tag)
        {
            var entity = _context.Tags.Find(tag.Name);
            if (_context.Tags.Find(tag.Name) != null)
            {
                return Response.BadRequest;
            }
            entity.Name = tag.Name;
            _context.SaveChanges();
            return Response.Updated;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}