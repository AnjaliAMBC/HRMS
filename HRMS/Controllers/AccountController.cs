using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HRMS.Models;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Net.Mail;
using System.Configuration;
using System.Net;

namespace Testhrms.Controllers
{
    public class AccountController : Controller
    {

        // Database context
        private readonly HrmstestEntities3 _dbContext;

        // Constructor to initialize database context
        public  AccountController()
        {
            _dbContext = new HrmstestEntities3(); // Replace YourDbContext with your actual DbContext class
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View("~/Views/Login/Login.cshtml");
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {

            // Check the username and password against your authentication system
            if (IsValidUser(model))
            {
                model.IsValidUser = true;
                // Authentication successful, redirect to the home page
                return Redirect("/Dashboard/Index");
            }
            else
            {
                model.InvalidLoginMessage = "Invalid username or password";
                model.IsValidUser = false;
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        private bool IsValidUser(LoginModel model)
        {
            if (model.EmployeeID != 0)
            {
                var isEmpExists = _dbContext.empinfoes.Where(x => x.empid == model.EmployeeID && x.password == model.Password).FirstOrDefault();
                if (isEmpExists != null)
                {
                    return true;
                }
            }

            if (!string.IsNullOrEmpty(model.EmailID))
            {
                var isEmpExists = _dbContext.empinfoes.Where(x => x.emailid == model.EmailID && x.password == model.Password).FirstOrDefault();
                if (isEmpExists != null)
                {
                    return true;
                }
            }

            return false;
        }

        public static string RenderPartialToString(Controller controller, string partialViewName, object model, ViewDataDictionary viewData, TempDataDictionary tempData)
        {
            ViewEngineResult result = ViewEngines.Engines.FindPartialView(controller.ControllerContext, partialViewName);

            if (result.View != null)
            {
                controller.ViewData.Model = model;
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    using (HtmlTextWriter output = new HtmlTextWriter(sw))
                    {
                        ViewContext viewContext = new ViewContext(controller.ControllerContext, result.View, viewData, tempData, output);
                        result.View.Render(viewContext, output);
                    }
                }

                return sb.ToString();
            }
            return String.Empty;
        }


        public ActionResult ForgotPassword(ForgotModel model)
        {
            var forgotPwdEmployee = _dbContext.empinfoes.Where(x => x.emailid == model.ForgotEmailID).FirstOrDefault();
            if (forgotPwdEmployee != null)
            {
                ForgotPasswordEmail(forgotPwdEmployee);
                model.IsEmailSent = true;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ForgotPasswordEmail(empinfo empModel)
        {
            var emailBody = "";
            var emailSubject = "";

            emailBody = RenderPartialToString(this, "_ForgotPasswordEmail", empModel, ViewData, TempData);

            using (MailMessage mm = new MailMessage(ConfigurationManager.AppSettings["SMTPUserName"], empModel.emailid))
            {
                mm.Subject = emailSubject;
                mm.Body = emailBody;

                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigurationManager.AppSettings["SMTPHost"];
                smtp.EnableSsl = true;
                NetworkCredential credentials = new NetworkCredential();
                credentials.UserName = ConfigurationManager.AppSettings["SMTPUserName"];
                credentials.Password = ConfigurationManager.AppSettings["SMTPPassword"];
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = credentials;
                smtp.Port = System.Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                smtp.Send(mm);
            }
            return Json(null);
        }

        [HttpGet]
        public ActionResult ResetPassword(int empid)
        {
            var model = new ResetModel();
            if (empid != 0)
            {
                var resetPasswordEmp = _dbContext.empinfoes.Where(x => x.empid == empid).FirstOrDefault();
                if (resetPasswordEmp != null)
                {
                    model.empInfo = resetPasswordEmp;
                }
            }
            return View("~/Views/Login/ResetPassword.cshtml", model);
        }

        public ActionResult UpdatePassword(UpdatePasswordModel model)
        {
            try
            {
                var resetPasswordEmp = _dbContext.empinfoes.Where(x => x.empid == model.Empid).FirstOrDefault();
                if (resetPasswordEmp != null)
                {
                    resetPasswordEmp.password = model.Password;
                    _dbContext.SaveChanges();
                    model.IsPasswordReset = true;
                    model.ResponseMessage = "Password reset successful!";
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                model.IsPasswordReset = false;
                model.ResponseMessage = "Password reset failed!";
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }
    }
}