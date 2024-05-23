using System.Web.Mvc;
using HRMS.Models;
using HRMS.Helpers;
using System.Web.Security;

namespace HRMS.Controllers
{
    public class ProfileController : Controller
    {

        private readonly HRMS_EntityFramework _dbContext;

        public ProfileController()
        {
            _dbContext = new HRMS_EntityFramework();
        }       


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            Session.Clear();

            return RedirectToAction("Login", "Account");
        }
    }
}