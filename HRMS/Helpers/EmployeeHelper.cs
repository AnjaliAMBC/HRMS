using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace HRMS.Helpers
{
    public class EmployeeHelper
    {
        private readonly HRMS_EntityFramework _dbContext;

        public EmployeeHelper(HRMS_EntityFramework context)
        {
            _dbContext = context;
        }

        public List<DropdownItem> GetDepartments()
        {
            var model = new List<DropdownItem>();
            try
            {
                var departments = _dbContext.Departments.ToList();
                model = departments.Select(x => new DropdownItem
                {
                    Id = x.DepartmentName,
                    Name = x.DepartmentName
                }).ToList();

            }

            catch (Exception ex)
            {
                ErrorHelper.CaptureError(ex);
            }
            return model;
        }

        //public List<DropdownItem> GetDesignations()
        //{
        //    var model = new List<DropdownItem>();
        //    try
        //    {
        //        var departments = _dbContext.Designations.ToList();
        //        model = departments.Select(x => new DropdownItem
        //        {
        //            Id = x.DepartmentName,
        //            Name = x.DepartmentName
        //        }).ToList();

        //    }

        //    catch (Exception ex)
        //    {
        //        ErrorHelper.CaptureError(ex);
        //    }
        //    return model;
        //}

    }
}