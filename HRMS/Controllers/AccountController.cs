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
using HRMS;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Security;

namespace Testhrms.Controllers
{
    public class AccountController : Controller
    {

        // Database context
        private readonly HRMS_Entities _dbContext;

        // Constructor to initialize database context
        public AccountController()
        {
            _dbContext = new HRMS_Entities(); // Replace YourDbContext with your actual DbContext class
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }
        [HttpPost]
        public async Task<ActionResult> Login(AccountModel loginModel)
        {
            // Validate reCAPTCHA
            var captchaResponse = loginModel.GCaptcha;
            var secretKey = "6LfVc7wpAAAAAPeeILyWCpmT8OAgM89K7SDLuQzG"; // Replace with your actual secret key
            var client = new HttpClient();
            var response = await client.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={captchaResponse}");
            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(jsonString);

            if (result.success != "true")
            {
                loginModel.InvalidLoginMessage = "reCAPTCHA validation failed.";
                loginModel.IsValidUser = false;
                return Json(loginModel, JsonRequestBehavior.AllowGet);
            }

            // Check the username and password against your authentication system
            if (IsValidUser(loginModel))
            {
                loginModel.IsValidUser = true;
                // Authentication successful, redirect to the home page
                return Redirect("/EmployeeAdmin/Index");
            }
            else
            {
                loginModel.InvalidLoginMessage = "Invalid username or password";
                loginModel.IsValidUser = false;
                return Json(loginModel, JsonRequestBehavior.AllowGet);
            }
        }


        private bool IsValidUser(AccountModel loginModel)
        {
            if (!string.IsNullOrWhiteSpace(loginModel.EmployeeID))
            {
                string hostName = Dns.GetHostName();
                var isEmpExists = _dbContext.Emplogins.Where(emp => emp.EmployeeID == loginModel.EmployeeID && emp.EmployeePassword == loginModel.Password).FirstOrDefault();
                if (isEmpExists != null)
                {
                    FormsAuthentication.SetAuthCookie(isEmpExists.EmployeeID.ToString(), loginModel.StaySignedIn);
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


        public ActionResult ForgotPassword(ForgotPasswordModel forgotModel)
        {
            var forgotPwdEmployee = _dbContext.Empinfoes.Where(emp => emp.OfficalEmailID == forgotModel.ForgotEmailID).FirstOrDefault();
            if (forgotPwdEmployee != null)
            {
                ForgotPasswordEmail(forgotPwdEmployee);
                forgotModel.IsEmailSent = true;
            }
            return Json(forgotModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ForgotPasswordEmail(Empinfo empInfoModel)
        {
            var emailBody = "";
            var emailSubject = "Password Reset Request";

            emailBody = RenderPartialToString(this, "_ForgotPasswordEmail", empInfoModel, ViewData, TempData);

            using (MailMessage mm = new MailMessage(ConfigurationManager.AppSettings["SMTPUserName"], empInfoModel.OfficalEmailID))
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
        public ActionResult ResetPassword(string employeeID)
        {
            var model = new ResetPasswordModel();
            if (!string.IsNullOrWhiteSpace(employeeID))
            {
                var resetPasswordEmp = _dbContext.Empinfoes.Where(emp => emp.EmployeeID == employeeID).FirstOrDefault();
                if (resetPasswordEmp != null)
                {
                    model.EmpInfo = resetPasswordEmp;
                }
            }
            return View("~/Views/Account/ResetPassword.cshtml", model);
        }

        public ActionResult UpdatePassword(UpdatePasswordModel updatePwdmodel)
        {
            try
            {
                var resetPasswordEmp = _dbContext.Emplogins.Where(emp => emp.EmployeeID == updatePwdmodel.Empid).FirstOrDefault();
                if (resetPasswordEmp != null)
                {
                    resetPasswordEmp.EmployeePassword = updatePwdmodel.Password;
                    _dbContext.SaveChanges();
                    updatePwdmodel.IsPasswordReset = true;
                    updatePwdmodel.ResponseMessage = "Password reset successful!";
                }

                return Json(updatePwdmodel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                updatePwdmodel.IsPasswordReset = false;
                updatePwdmodel.ResponseMessage = "Password reset failed!";
                return Json(updatePwdmodel, JsonRequestBehavior.AllowGet);
            }
        }
    }
}