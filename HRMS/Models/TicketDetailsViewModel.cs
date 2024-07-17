using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    public class TicketDetailsViewModel
    {
        public IT_Ticket TicketViewModel { get; set; } = new IT_Ticket();
        public List<emp_info> ITEmployees { get; set; } = new List<emp_info>();
    }
}