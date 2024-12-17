using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    public class Graph
    {
        public string label { get; set; }
        public decimal y { get; set; }
        public string indexLabel { get; set; }
    }

    public class WeekDateModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }

}