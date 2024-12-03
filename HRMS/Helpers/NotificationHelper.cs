using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Helpers
{
    public class NotificationHelper
    {
        public static string GetNotificationTitle(Notification notification)
        {
            if (notification == null) return "Notification";

            string title = string.Empty;

            switch (notification.NotificationType)
            {
                case "Leave":
                    switch (notification.Status)
                    {
                        case "Approved":
                            title = $"Your leave request was approved by {notification.NotificationFromName}.";
                            break;
                        case "Rejected":
                            title = $"Your leave request was rejected by {notification.NotificationFromName}.";
                            break;
                        case "Submitted":
                            title = $"{notification.NotificationFromName} has submitted a leave request for approval.";
                            break;
                        case "Updated":
                            title = $"Your leave request was updated by {notification.NotificationFromName}.";
                            break;
                        case "Cancelled":
                            title = $"Your leave request was cancelled by {notification.NotificationFromName}.";
                            break;
                        default:
                            title = $"Leave request status changed by {notification.NotificationFromName}.";
                            break;
                    }
                    break;

                case "Ticket":
                    switch (notification.Status)
                    {
                        case "Resolved":
                            title = $"Ticket #{notification.ReferenceNumber} has been resolved.";
                            break;
                        case "Re-Open":
                            title = $"Ticket #{notification.ReferenceNumber} has been re-opened.";
                            break;
                        case "Closed":
                            title = $"Ticket #{notification.ReferenceNumber} has been closed.";
                            break;
                        case "Acknowledged":
                            title = $"Ticket #{notification.ReferenceNumber} has been acknowledged.";
                            break;
                        case "Submitted":
                            title = $"Ticket #{notification.ReferenceNumber} has been submitted.";
                            break;
                        case "Cancelled":
                            title = $"Ticket #{notification.ReferenceNumber} was cancelled.";
                            break;
                        default:
                            title = $"New ticket update for #{notification.ReferenceNumber}.";
                            break;
                    }
                    break;

                case "Birthday":
                    if (notification.RepiedSno != 0)
                    {
                        title = $"{notification.Comments} from {notification.NotificationFromName}!";
                    }
                    else
                    {
                        title = $"Birthday wish from {notification.NotificationFromName}!";
                    }

                    break;

                case "Anniversary":
                    if (notification.RepiedSno != 0)
                    {
                        title = $"{notification.Comments} from {notification.NotificationFromName}!";
                    }
                    else
                    {
                        title = $"Anniversary wish from {notification.NotificationFromName}!";
                    }
                    break;

                case "NewJoining":
                    if (notification.RepiedSno != 0)
                    {
                        title = $"{notification.Comments} from {notification.NotificationFromName}!";
                    }
                    else
                    {
                        title = $"Welcome to the team from {notification.NotificationFromName}!";
                    }
                    break;

                case "Remainder":
                    switch (notification.Status)
                    {
                        case "CheckIn":
                            title = "Reminder: Please check-in.";
                            break;
                        case "CheckOut":
                            title = "Reminder: Please check-out.";
                            break;
                        default:
                            title = "Reminder notification.";
                            break;
                    }
                    break;

                default:
                    title = "Notification";
                    break;
            }

            return title;
        }


    }
}