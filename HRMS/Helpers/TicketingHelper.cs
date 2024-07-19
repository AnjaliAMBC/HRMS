using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Helpers
{
    public static class TicketingHelper
    {
        public static string GetPriorityColor(string priority)
        {
            switch (priority.ToLower())
            {
                case "high":
                    return "res-emp-ticketlisting-color-red";
                case "mid":
                    return "res-emp-ticketlisting-color-orange";
                case "low":
                    return "res-emp-ticketlisting-color-green";
                default:
                    return ""; // Default or unknown priority
            }
        }

        // Method to return CSS class based on ticket status
        public static string GetTicketStatusCssClass(string status)
        {
            switch (status.ToLower())
            {
                case "open":
                    return "ithrticket-status-open";
                case "closed":
                    return "ithrticket-status-closed";
                case "resolved":
                    return "ticket-status-pending";
                case "re-open":
                    return "ticket-status-re-open";
                case "cancelled":
                    return "ticket-status-cancelled";
                default:
                    return ""; 
            }
        }
       
            public static string GetTicketPriorityDashCssClass(string status)
        {
            switch (status.ToLower())
            {
                case "low":
                    return "badge-low";
                case "mid":
                    return "badge-mid";
                case "high":
                    return "badge-danger";                
                default:
                    return "";
            }
        }
    }
}