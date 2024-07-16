using HRMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class AdminHrTicketingController : Controller
    {
        private readonly HRMS_EntityFramework _dbContext;
        // GET: AdminHrTicketing
        public ActionResult Index()
        {
            var tickets = _dbContext.IT_Ticket.ToList();
            return View(tickets);
        }

        public ActionResult EmpTicketList()
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            

            var employeeId = cuserContext.EmpInfo.EmployeeID; // Retrieve this from session or authentication context
            var employeeTickets = _dbContext.IT_Ticket.Where(t => t.EmployeeID == employeeId).ToList();
            return View(employeeTickets);
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IT_Ticket ticket, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = System.IO.Path.GetFileName(file.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                    file.SaveAs(path);
                    ticket.AttatchimageFile = "/Uploads/" + fileName;
                }

                ticket.Created_date = DateTime.Now;
                ticket.Status = "Open";
                _dbContext.IT_Ticket.Add(ticket);
                _dbContext.SaveChanges();

                // Generate formatted ticket number
                //ticket.FormattedTicketNumber = $"{ticket.TicketNo}-{ticket.TicketId}";
                //db.Entry(ticket).State = EntityState.Modified;
                //db.SaveChanges();

            }

            return View(ticket);
        }

        // GET: Ticket/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            IT_Ticket ticket = _dbContext.IT_Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Ticket/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            IT_Ticket ticket = _dbContext.IT_Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Ticket/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IT_Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                //_dbContext.Entry(ticket).State = .Modified;
                //_dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticket);
        }

        // GET: Ticket/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            IT_Ticket ticket = _dbContext.IT_Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Ticket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IT_Ticket ticket = _dbContext.IT_Ticket.Find(id);
            _dbContext.IT_Ticket.Remove(ticket);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AdminHrTicket()
        {
            return View("~/Views/AdminDashboard/AdminHrTicketing.cshtml");
        }
        public ActionResult AdminHrTicketOpenClose()
        {
             return View("~/Views/AdminDashboard/AdminHrTicketOpenCloseView.cshtml");
        }
    }
}