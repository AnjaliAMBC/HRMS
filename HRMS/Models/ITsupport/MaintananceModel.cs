using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.ITsupport
{
    public class MaintananceModel
    {
        public List<emp_info> ITEmployees { get; set; } = new List<emp_info>();
        public List<emp_info> Employees { get; set; } = new List<emp_info>();

        public List<IT_Maintenance> monthlyschedules { get; set; } = new List<IT_Maintenance>();

        public IT_Maintenance EditableRecord = new IT_Maintenance();

        public emp_info SelectedEmp { get; set; } = new emp_info();

        public List<int> Years { get; set; } = new List<int>();

    }
}