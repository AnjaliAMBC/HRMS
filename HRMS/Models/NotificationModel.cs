using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    public class NotificationModel
    {
        public string NotificationFromName { get; set; }
        public string NotificationFromID { get; set; }
        public string NotificationToName { get; set; }
        public string NotificationToID { get; set; }
        public string NotificationType { get; set; }
        public string Comments { get; set; }
    }

}