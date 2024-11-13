using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.Admin
{
    public class AdminJobModel : SiteContextModel
    {
        public List<JobDetail> alljobListings { get; set; }
        public JobDetail EditJob { get; set; } = new JobDetail();
    }

}