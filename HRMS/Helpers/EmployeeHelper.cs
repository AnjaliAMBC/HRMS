using HRMS.Models;
using HRMS.Models.Employee;
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

        public List<DropdownItem> GetDesignations()
        {
            var model = new List<DropdownItem>();
            try
            {
                var departments = _dbContext.Designations.ToList();
                model = departments.Select(x => new DropdownItem
                {
                    Id = x.Designation1,
                    Name = x.Designation1
                }).ToList();

            }

            catch (Exception ex)
            {
                ErrorHelper.CaptureError(ex);
            }
            return model;
        }

        public List<DropdownItem> GetClients()
        {
            var model = new List<DropdownItem>();
            try
            {
                var departments = _dbContext.Clients.ToList();
                model = departments.Select(x => new DropdownItem
                {
                    Id = x.ClientName,
                    Name = x.ClientName
                }).ToList();

            }

            catch (Exception ex)
            {
                ErrorHelper.CaptureError(ex);
            }
            return model;
        }

        public List<DropdownItem> GetLeaveManagers()
        {
            var model = new List<DropdownItem>();
            try
            {
                var leaveManagers = _dbContext.LeaveRMs.Where(emp => emp.IsLeaveRM == true).ToList();
                model = leaveManagers.Select(x => new DropdownItem
                {
                    Id = x.Name,
                    Name = x.Name,
                }).ToList();
            }

            catch (Exception ex)
            {
                ErrorHelper.CaptureError(ex);
            }
            return model;
        }

        public List<DropdownItem> GetReportingManagers()
        {
            var model = new List<DropdownItem>();
            try
            {
                var reportingManagers = _dbContext.LeaveRMs.Where(emp => emp.IsReporingM == true).ToList();
                model = reportingManagers.Select(x => new DropdownItem
                {
                    Id = x.Name,
                    Name = x.Name,
                }).ToList();
            }

            catch (Exception ex)
            {
                ErrorHelper.CaptureError(ex);
            }
            return model;
        }


        public static EmployeeDisplayModel GetEmployeeDisplayInfo(string empID, emp_info empployeeInfo = null)
        {
            EmployeeDisplayModel displayModel = new EmployeeDisplayModel();

            using (var dbContext = new HRMS_EntityFramework())
            {
                var empInfo = empployeeInfo == null ? dbContext.emp_info.Where(x => x.EmployeeID == empID).FirstOrDefault() : empployeeInfo;

                if (empInfo == null)
                    return displayModel;

                if (!string.IsNullOrWhiteSpace(empInfo.imagepath))
                {
                    // Construct the image URL
                    string empImageUrl = System.Configuration.ConfigurationManager.AppSettings["EmpImagesFolder"]
                                         + "/" + empInfo.imagepath
                                         + "?" + System.DateTime.Now;

                    displayModel.ImageSrc = empImageUrl;
                }
                else
                {
                    // Generate the initials if there is no image
                    string[] names = empInfo.EmployeeName?.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (names != null && names.Length > 0)
                    {
                        string firstName = names[0];
                        string lastName = names.Length > 1 ? names[names.Length - 1] : string.Empty;

                        char firstLetterOfFirstName = char.ToUpper(firstName[0]);
                        char firstLetterOfLastName = !string.IsNullOrEmpty(lastName) ? char.ToUpper(lastName[0]) : '\0';

                        displayModel.EmpShortName = firstLetterOfFirstName.ToString() + (firstLetterOfLastName != '\0' ? firstLetterOfLastName.ToString() : string.Empty);
                    }
                }

                displayModel.EmpInfo = empInfo;
            }

            return displayModel;
        }
    }
}