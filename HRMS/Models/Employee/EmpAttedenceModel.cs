using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.Employee
{
    public class EmpAttedenceModel : SiteContextModel
    {
        public List<tbld_ambclogininformation> AttedenceList { get; set; } = new List<tbld_ambclogininformation>();

        public DateTime startWeek { get; set; }

        public DateTime EndWeek { get; set; }

        public List<DateTime> AllDates { get; set; } = new List<DateTime>();
    }
}