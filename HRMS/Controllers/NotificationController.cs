using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Models;

namespace HRMS.Controllers
{
    public class NotificationController : BaseController
    {
        private readonly HRMS_EntityFramework _dbContext;
        public NotificationController()
        {
            _dbContext = new HRMS_EntityFramework();
        }

        public JsonResult SendNotification(NotificationModel model)
        {
            try
            {
                var newNotification = new Notification
                {
                    NotificationDate = DateTime.Now,
                    NotificationFromName = model.NotificationFromName,
                    NotificationFromID = model.NotificationFromID,
                    NotificationToName = model.NotificationToName,
                    NotificationToID = model.NotificationToID,
                    NotificationType = model.NotificationType,
                    Status = "Sent",
                    Comments = model.Comments,
                    CreatedDate = DateTime.Now
                };

                _dbContext.Notifications.Add(newNotification);
                _dbContext.SaveChanges();

                return Json(new { success = true, message = model.NotificationType + " message sent successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error sending message: " + ex.Message });
            }
        }
    }
}