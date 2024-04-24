using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    public class SiteContext
    {
        public emplogin LoginInfo { get; set; } = new emplogin();
        public emp_info EmpInfo { get; set; } = new emp_info();
    }
}