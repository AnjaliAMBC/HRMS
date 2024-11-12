using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.Employee
{
    public class EmpJobModel
    {
        public List<JobDetail> jobdetail  = new List<JobDetail>();

        public JobDetail jobInfo { get; set; }

        public emp_info EmpInfo { get; set; } = new emp_info();
    }
}