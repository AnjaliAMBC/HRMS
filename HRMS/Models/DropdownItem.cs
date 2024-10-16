using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    public class DropdownItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class MonthInfo
    {
        public int Number { get; set; }
        public string Name { get; set; }

        public MonthInfo(int number, string name)
        {
            Number = number;
            Name = name;
        }
    }
}