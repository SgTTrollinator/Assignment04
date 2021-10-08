using System;
using Assignment4.Core;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            throw new NotImplementedException();
        }

        public Response Delete(int tagId, bool force = false)
        {
            throw new NotImplementedException();
        }

        public TagDTO Read(int tagId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TagDTO> ReadAll()
        {
            throw new NotImplementedException();
        }

        public Response Update(TagUpdateDTO tag)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}