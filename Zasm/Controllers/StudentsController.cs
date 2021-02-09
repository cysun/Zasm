using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Zasm.Models;
using Zasm.Security.Constants;
using Zasm.Services;

namespace Zasm.Controllers
{
    [Authorize(Policy = Policy.IsAdmin)]
    public class StudentsController : Controller
    {
        private readonly StudentService _studentService;
        private readonly ClassService _classService;

        private readonly IMapper _mapper;
        private ILogger<StudentsController> _logger;

        public StudentsController(StudentService studentService, ClassService classService,
            IMapper mapper, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _classService = classService;
            _mapper = mapper;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var studentViewModels = _mapper.Map<List<StudentViewModel>>(_studentService.GetStudents());
            foreach (var studentViewModel in studentViewModels)
            {
                studentViewModel.LessonsTotal = studentViewModel.Payments.Select(p => p.Lessons).Sum();
                studentViewModel.PaymentsTotal = studentViewModel.Payments.Select(p => p.Amount).Sum();
            }
            return View(studentViewModels);
        }

        public IActionResult View(int id)
        {
            var student = _studentService.GetStudent(id);
            if (student == null)
                return View("Error", new ErrorViewModel
                {
                    Message = "This student does not exist."
                });

            var studentViewModel = _mapper.Map<StudentViewModel>(student);
            studentViewModel.LessonsTotal = student.Payments.Select(p => p.Lessons).Sum();
            studentViewModel.PaymentsTotal = student.Payments.Select(p => p.Amount).Sum();

            return View(studentViewModel);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Classes = _classService.GetClasses().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
            return View(new StudentInputModel());
        }

        [HttpPost]
        public IActionResult Add(StudentInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var student = _mapper.Map<Student>(input);
            _studentService.AddStudent(student);
            _studentService.SaveChanges();

            _logger.LogInformation("{user} added student {student}", User.Identity.Name, student.Id);

            return RedirectToAction("View", new { id = student.Id });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var student = _studentService.GetStudent(id);
            if (student == null)
                return View("Error", new ErrorViewModel
                {
                    Message = "This student does not exist."
                });

            ViewBag.Classes = _classService.GetClasses().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
            return View(_mapper.Map<StudentInputModel>(student));
        }

        [HttpPost]
        public IActionResult Edit(int id, StudentInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var student = _studentService.GetStudent(id);
            if (student == null)
                return View("Error", new ErrorViewModel
                {
                    Message = "This student does not exist."
                });

            _mapper.Map(input, student);
            _studentService.SaveChanges();

            _logger.LogInformation("{user} edited student {student}", User.Identity.Name, id);

            return RedirectToAction("View", new { id = student.Id });
        }
    }
}

namespace Zasm.Models
{
    public class StudentInputModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Birth Year")]
        public int BirthYear { get; set; } = DateTime.Today.Year;

        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [MaxLength(32)]
        [Display(Name = "Parent Name")]
        public string ParentName { get; set; }

        [MaxLength(255)]
        [Display(Name = "Parent Email")]
        public string ParentEmail { get; set; }

        [Display(Name = "Active")]
        public bool IsActivie { get; set; } = true;
    }

    public class StudentViewModel : Student
    {
        public int PaymentsTotal { get; set; }
        public int LessonsTotal { get; set; }
        public int LessonsLeft => LessonsTotal - Attendances.Count;
    }
}
