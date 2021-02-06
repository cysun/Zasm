using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zasm.Models;

namespace Zasm.Services
{
    public class LessonService
    {
        private readonly AppDbContext _db;

        public LessonService(AppDbContext db)
        {
            _db = db;
        }

        public List<Lesson> GetLessons()
        {
            return _db.Lessons.Include(l => l.Class).OrderByDescending(l => l.DateTime).ToList();
        }

        public Lesson GetLesson(int id)
        {
            return _db.Lessons.Where(l => l.Id == id)
                .Include(l => l.Class)
                .Include(l => l.Attendances).ThenInclude(a => a.Student)
                .SingleOrDefault();
        }

        public void AddLesson(Lesson lesson) => _db.Lessons.Add(lesson);

        public void DeleteLesson(Lesson lesson) => _db.Lessons.Remove(lesson);

        public void AddAttendance(int lessonId, int studentId)
        {
            var attendance = _db.Attendances
                .Where(a => a.LessonId == lessonId && a.StudentId == studentId)
                .SingleOrDefault();
            if (attendance == null)
            {
                _db.Attendances.Add(new Attendance
                {
                    LessonId = lessonId,
                    StudentId = studentId
                });
                _db.SaveChanges();
            }
        }

        public void RemoveAttendance(int lessonId, int studentId)
        {
            var attendance = _db.Attendances
                .Where(a => a.LessonId == lessonId && a.StudentId == studentId)
                .SingleOrDefault();
            if (attendance != null)
            {
                _db.Attendances.Remove(attendance);
                _db.SaveChanges();
            }
        }

        public void SaveChanges() => _db.SaveChanges();
    }
}
