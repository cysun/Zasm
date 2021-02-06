using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zasm.Models;

namespace Zasm.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _db;

        public PaymentService(AppDbContext db)
        {
            _db = db;
        }

        public List<Payment> GetPayments()
        {
            return _db.Payments.Include(p => p.Student).OrderByDescending(p => p.DateTime).ToList();
        }

        public Payment GetPayment(int id)
        {
            return _db.Payments.Where(p => p.Id == id).Include(p => p.Student).SingleOrDefault();
        }

        public void AddPayment(Payment payment) => _db.Payments.Add(payment);

        public void DeletePayment(Payment payment) => _db.Payments.Remove(payment);

        public void SaveChanges() => _db.SaveChanges();
    }
}
