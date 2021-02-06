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
    public class PaymentsController : Controller
    {
        private readonly PaymentService _paymentService;
        private readonly StudentService _studentService;

        private readonly IMapper _mapper;
        private ILogger<PaymentsController> _logger;

        public PaymentsController(PaymentService paymentService, StudentService studentService,
            IMapper mapper, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _studentService = studentService;
            _mapper = mapper;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var payments = _paymentService.GetPayments();
            return View(payments);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Students = _studentService.GetStudents().Select(s => new SelectListItem
            {
                Text = $"{s.Name} ({s.ParentName})",
                Value = s.Id.ToString()
            });
            return View(new PaymentInputModel());
        }

        [HttpPost]
        public IActionResult Add(PaymentInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var payment = _mapper.Map<Payment>(input);
            _paymentService.AddPayment(payment);
            _paymentService.SaveChanges();

            _logger.LogInformation("{user} added payment {payment}", User.Identity.Name, payment.Id);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var payment = _paymentService.GetPayment(id);
            if (payment == null)
                return View("Error", new ErrorViewModel
                {
                    Message = "This payment does not exist."
                });

            _paymentService.DeletePayment(payment);
            _paymentService.SaveChanges();

            _logger.LogInformation("{user} deleted payment {payment}", User.Identity.Name, id);

            return RedirectToAction("Index");
        }
    }
}

namespace Zasm.Models
{
    public class PaymentInputModel
    {
        public int Id { get; set; }

        [Display(Name = "Time")]
        [DataType(DataType.Date)]
        public DateTime DateTime { get; set; } = DateTime.Today;

        [Display(Name = "Student")]
        public int StudentId { get; set; }

        public int Amount { get; set; }

        public int Lessons { get; set; }
    }
}
