using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Zasm.Models;
using Zasm.Services;

namespace Zasm.Controllers
{
    public class LessonsController : Controller
    {
        private readonly LessonService _lessonService;
        private readonly ClassService _classService;

        private readonly IMapper _mapper;
        private ILogger<LessonsController> _logger;

        public LessonsController(LessonService lessonService, ClassService classService,
            IMapper mapper, ILogger<LessonsController> logger)
        {
            _lessonService = lessonService;
            _classService = classService;
            _mapper = mapper;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var lessons = _lessonService.GetLessons();
            return View(lessons);
        }

        public IActionResult View(int id)
        {
            var lesson = _lessonService.GetLesson(id);
            if (lesson == null)
                return View("Error", new ErrorViewModel
                {
                    Message = "This lesson does not exist."
                });

            var lessonViewModel = _mapper.Map<LessonViewModel>(lesson);

            var students = _classService.GetClass(lesson.ClassId).Students;
            var studentAttended = lesson.Attendances.Select(a => a.StudentId).ToHashSet();
            lessonViewModel.AttendanceViewModels = students.Select(s => new AttendanceViewModel
            {
                Lesson = lesson,
                Student = s,
                Attended = studentAttended.Contains(s.Id)
            }).ToList();

            return View(lessonViewModel);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Classes = _classService.GetClasses().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
            return View(new LessonInputModel());
        }

        [HttpPost]
        public IActionResult Add(LessonInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var lesson = _mapper.Map<Lesson>(input);
            _lessonService.AddLesson(lesson);
            _lessonService.SaveChanges();

            _logger.LogInformation("{user} added lesson {lesson}", User.Identity.Name, lesson.Id);

            return RedirectToAction("View", new { id = lesson.Id });
        }

        public IActionResult Delete(int id)
        {
            var lesson = _lessonService.GetLesson(id);
            if (lesson == null)
                return View("Error", new ErrorViewModel
                {
                    Message = "This lesson does not exist."
                });

            _lessonService.DeleteLesson(lesson);
            _lessonService.SaveChanges();

            _logger.LogInformation("{user} deleted lesson {lesson}", User.Identity.Name, id);

            return RedirectToAction("Index");
        }

        [HttpPut("Lessons/{lessonId}/Students/{studentId}")]
        public IActionResult AddAttendance(int lessonId, int studentId)
        {
            _lessonService.AddAttendance(lessonId, studentId);

            _logger.LogInformation("{user} added student {student} to lesson {lesson}",
                User.Identity.Name, studentId, lessonId);

            return Ok();
        }

        [HttpDelete("Lessons/{lessonId}/Students/{studentId}")]
        public IActionResult RemoveAttendance(int lessonId, int studentId)
        {
            _lessonService.RemoveAttendance(lessonId, studentId);

            _logger.LogInformation("{user} removed student {student} to lesson {lesson}",
                User.Identity.Name, studentId, lessonId);

            return Ok();
        }

    }
}

namespace Zasm.Models
{
    public class LessonInputModel
    {
        public int Id { get; set; }

        [Display(Name = "Time")]
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; } = DateTime.Today;

        [Display(Name = "Length")]
        public int Minutes { get; set; } = 90;

        [Display(Name = "Class")]
        public int ClassId { get; set; }
    }

    public class AttendanceViewModel
    {
        public Lesson Lesson { get; set; }
        public Student Student { get; set; }
        public bool Attended { get; set; }
    }

    public class LessonViewModel
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int Minutes { get; set; }
        public Class Class { get; set; }

        public List<AttendanceViewModel> AttendanceViewModels { get; set; }
    }
}
