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
using System.Text.RegularExpressions;

namespace HRMS.Controllers
{
    //using HRMS.Helpers;

    public class AccountController : BaseController
    {
        // Database context
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public AccountController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            Session["SiteContext"] = null;
            return View("~/Views/Account/Login.cshtml");
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(AccountModel loginModel)
        {

            if (IsValidUser(loginModel))
            {
                FormsAuthentication.SetAuthCookie(loginModel.EmailID, false);
                loginModel.IsValidUser = true;
                return Json(loginModel, JsonRequestBehavior.AllowGet);
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
            var SiteContext = new SiteContextModel();
            if (!string.IsNullOrWhiteSpace(loginModel.EmployeeID))
            {
                loginModel.EmployeeID = loginModel.EmployeeID.Trim();
                loginModel.Password = loginModel.Password.Trim();

                var isEmpExists = _dbContext.emplogins
                    .Where(emp => emp.EmployeeID == loginModel.EmployeeID && emp.Password == loginModel.Password)
                    .FirstOrDefault();

                if (isEmpExists != null && isEmpExists.EmployeeStatus == "Active")
                {
                    loginModel.IsUser = true;
                    var currentEmployee = _dbContext.emp_info
                        .Where(emp => emp.EmployeeID == loginModel.EmployeeID)
                        .FirstOrDefault();

                    var empClients = _dbContext.EmployeeBasedClients.Where(x => x.EmployeeID == loginModel.EmployeeID).OrderBy(x => x.CreatedDate).ToList();
                    if (empClients != null)
                    {
                        SiteContext.EmpBasedClients = empClients;
                    }
                    else
                    {
                        SiteContext.EmpBasedClients = new List<EmployeeBasedClient>();
                    }

                    SiteContext.LoginInfo = isEmpExists;
                    SiteContext.EmpInfo = currentEmployee;
                    Session["SiteContext"] = SiteContext;

                    FormsAuthentication.SetAuthCookie(isEmpExists.EmployeeID.ToString(), loginModel.StaySignedIn);
                    return true;
                }
            }

            if (!string.IsNullOrWhiteSpace(loginModel.EmailID))
            {

                loginModel.EmailID = loginModel.EmailID.Trim();
                loginModel.Password = loginModel.Password.Trim();

                var isEmpExists = _dbContext.emplogins
                    .Where(emp => emp.EmployeeEmail == loginModel.EmailID && emp.Password == loginModel.Password &&
                                  (emp.EmployeeRole.Contains("HR Admin") || emp.EmployeeRole.Contains("Super Admin") || emp.EmployeeRole.Contains("IT Admin") || emp.EmployeeRole.Contains("Account Admin") || emp.EmployeeRole.Contains("Rec") || emp.EmployeeRole.Contains("TimeAdmin")))
                    .FirstOrDefault();

                if (isEmpExists != null && isEmpExists.EmployeeStatus == "Active") // Check if employee status is active
                {
                    var currentEmployee = _dbContext.emp_info
                        .Where(emp => emp.OfficalEmailid == loginModel.EmailID)
                        .FirstOrDefault();

                    var empClients = _dbContext.EmployeeBasedClients.Where(x => x.EmployeeID == loginModel.EmployeeID).OrderBy(x => x.CreatedDate).ToList();
                    if (empClients != null)
                    {
                        SiteContext.EmpBasedClients = empClients;
                    }
                    else
                    {
                        SiteContext.EmpBasedClients = new List<EmployeeBasedClient>();
                    }


                    SiteContext.LoginInfo = isEmpExists;
                    SiteContext.EmpInfo = currentEmployee;

                    if (isEmpExists.EmployeeRole.ToLowerInvariant().Contains("it admin"))
                    {
                        loginModel.IsITAdmin = true;
                        SiteContext.IsITAdmin = true;
                        SiteContext.IsAdmin = false;
                    }

                    if (isEmpExists.EmployeeRole.ToLowerInvariant().Contains("super admin"))
                    {
                        loginModel.IsSuperAdmin = true;
                        loginModel.IsAdmin = true;
                        SiteContext.IsAdmin = true;
                        SiteContext.IsSuperAdmin = true;
                    }

                    if (isEmpExists.EmployeeRole.ToLowerInvariant().Contains("hr admin"))
                    {
                        loginModel.IsAdmin = true;
                        SiteContext.IsAdmin = true;
                    }

                    if (isEmpExists.EmployeeRole.ToLowerInvariant().Contains("account admin"))
                    {
                        loginModel.IsAccountAdmin = true;
                        SiteContext.IsAccountAdmin = true;
                    }

                    if (isEmpExists.EmployeeRole.ToLowerInvariant().Contains("rec"))
                    {
                        loginModel.IsHiringAdmin = true;
                        SiteContext.IsHiringAdmin = true;
                    }

                    if (isEmpExists.EmployeeRole.ToLowerInvariant().Contains("timeadmin"))
                    {
                        loginModel.IsTimeAdmin = true;
                        SiteContext.IsTimeAdmin = true;
                    }


                    Session["SiteContext"] = SiteContext;

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

        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPasswordModel forgotModel)
        {
            try
            {
                if (forgotModel == null)
                    return null;
                forgotModel.JsonResponse = new JsonResponse();
                var forgotPwdEmployee = _dbContext.emplogins.Where(emp => emp.EmployeeEmail == forgotModel.ForgotEmailID).FirstOrDefault();
                if (forgotPwdEmployee != null)
                {
                    ForgotPasswordEmail(forgotPwdEmployee, forgotModel);
                    forgotModel.IsEmailSent = true;
                    forgotModel.JsonResponse.Message = "Reset password link sent to email!";
                    forgotModel.JsonResponse.StatusCode = 200;
                }
                else
                {
                    forgotModel.IsEmailSent = false;
                    forgotModel.JsonResponse.Message = "Email not sent. Looks like Email id is incorrect!";
                    forgotModel.JsonResponse.StatusCode = 404;
                }
                return Json(forgotModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(forgotModel, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public JsonResult ForgotPasswordEmail(emplogin empInfoModel, ForgotPasswordModel forgotModel)
        {
            try
            {
                var emailBody = "";
                var emailSubject = "Reset Password";
                emailBody = RenderPartialToString(this, "_ForgotPasswordEmail", empInfoModel, ViewData, TempData);

                using (MailMessage mm = new MailMessage(ConfigurationManager.AppSettings["SMTPUserName"], empInfoModel.EmployeeEmail))
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
            catch (Exception ex)
            {
                forgotModel.JsonResponse.Message = ex.Message;
                forgotModel.JsonResponse.StatusCode = 500;
                forgotModel.IsEmailSent = false;
                return Json(forgotModel, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string empid)
        {
            var model = new ResetPasswordModel();
            if (!string.IsNullOrWhiteSpace(empid))
            {
                var resetPasswordEmp = _dbContext.emplogins.Where(emp => emp.EmployeeID == empid).FirstOrDefault();
                if (resetPasswordEmp != null)
                {
                    model.EmpInfo = resetPasswordEmp;
                }
            }
            return View("~/Views/Account/ResetPassword.cshtml", model);
        }

        [AllowAnonymous]
        public ActionResult UpdatePassword(UpdatePasswordModel updatePwdmodel)
        {
            try
            {
                var resetPasswordEmp = _dbContext.emplogins.Where(emp => emp.EmployeeID == updatePwdmodel.Empid).FirstOrDefault();
                if (resetPasswordEmp != null)
                {
                    resetPasswordEmp.Password = updatePwdmodel.Password;
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

        [HttpPost]
        public JsonResult ChangePassword(string currentPassword, string newPassword, string confirmPassword, string empid)
        {
            var result = new Dictionary<string, string>();
            bool success = true;

            if (success)
            {
                // Fetch the user from the database
                var changePasswordEmp = _dbContext.emplogins.SingleOrDefault(u => u.EmployeeID == empid);
                if (changePasswordEmp != null)
                {
                    if (changePasswordEmp.Password == currentPassword)
                    {
                        changePasswordEmp.Password = newPassword;
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        result["currentPassword"] = "Current password is incorrect.";
                        success = false;
                    }

                }
            }

            return Json(new { success = success, errors = result });
        }

        private bool IsValidPassword(string password)
        {
            // Check if the password meets the criteria
            var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return passwordRegex.IsMatch(password);
        }

        private bool VerifyPassword(string storedPassword, string inputPassword)
        {
            return storedPassword == inputPassword;
        }

        private string HashPassword(string password)
        {

            return password;
        }

    }
}