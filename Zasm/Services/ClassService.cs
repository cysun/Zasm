using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zasm.Models;

namespace Zasm.Services
{
    public class ClassService
    {
        private readonly AppDbContext _db;

        public ClassService(AppDbContext db)
        {
            _db = db;
        }

        public List<Class> GetClasses() => _db.Classes.OrderBy(c => c.Id).ToList();

        public Class GetClass(int id)
        {
            var c = _db.Classes.Where(c => c.Id == id).Include(c => c.Students).SingleOrDefault();

            if (c != null) c.Students = c.Students.OrderBy(s => s.Name).ToList();

            return c;
        }
    }
}
