using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.ITsupport
{
    public class MaintananceModel
    {
        public List<emp_info> Employees { get; set; } = new List<emp_info>();
    }
}