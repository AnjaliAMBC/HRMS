using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    public class TicketingModel : SiteContextModel
    {
        public List<IT_Ticket> empTickets { get; set; } = new List<IT_Ticket>();
    }
}