using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Zasm.Models
{
    public class Class
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        public List<Student> Students { get; set; }
    }

    public class Student
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        public int? BirthYear { get; set; }

        public int ClassId { get; set; }
        public Class Class { get; set; }

        [MaxLength(32)]
        public string ParentName { get; set; }

        [MaxLength(255)]
        public string ParentEmail { get; set; }

        public bool IsActivie { get; set; } = true;

        public List<Attendance> Attendances { get; set; }
        public List<Payment> Payments { get; set; }
    }

    public class Lesson
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;
        public int Minutes { get; set; } = 90;

        public int ClassId { get; set; }
        public Class Class { get; set; }

        public List<Attendance> Attendances { get; set; }
    }

    [Table("Attendances")]
    public class Attendance
    {
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }
    }

    public class Payment
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateTime { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int Amount { get; set; }

        public int Lessons { get; set; }
    }
}
