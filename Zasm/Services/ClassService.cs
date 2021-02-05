using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
