using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zasm.Models;

namespace Zasm.Services
{
    public class StudentService
    {
        private readonly AppDbContext _db;

        public StudentService(AppDbContext db)
        {
            _db = db;
        }

        public List<Student> GetStudents()
        {
            return _db.Students
                .Include(s => s.Class)
                .Include(s => s.Attendances).ThenInclude(a => a.Lesson)
                .Include(s => s.Payments)
                .OrderBy(s => s.IsActivie).ToList();
        }

        public Student GetStudent(int id)
        {
            var student = _db.Students.Where(s => s.Id == id)
                .Include(s => s.Class)
                .Include(s => s.Attendances).ThenInclude(a => a.Lesson)
                .Include(s => s.Payments)
                .SingleOrDefault();

            if (student != null)
            {
                student.Attendances = student.Attendances.OrderByDescending(a => a.Lesson.DateTime).ToList();
                student.Payments = student.Payments.OrderByDescending(p => p.DateTime).ToList();
            }

            return student;
        }

        public void AddStudent(Student student) => _db.Students.Add(student);

        public void SaveChanges() => _db.SaveChanges();
    }
}
