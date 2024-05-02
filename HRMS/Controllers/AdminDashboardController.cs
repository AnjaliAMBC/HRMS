using HRMS.Models;
using HRMS.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Helpers;
using Newtonsoft.Json;
using System.Data;
using System.IO;
using OfficeOpenXml;

namespace HRMS.Controllers
{
    public class AdminDashboardController : Controller
    {
        // Database context
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public AdminDashboardController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }

        // GET: AdminDashboard
        //[CustomAuthorize(Roles = "HR Admin")]
        public ActionResult Dashboard()
        {
            var model = new DashboardViewModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;
            return View("~/Views/AdminDashboard/Dashboard.cshtml", model);
        }

        //[CustomAuthorize(Roles = "HR Admin")]
        public ActionResult EmployeeManagement()
        {
            var model = new EmployeeManagementViewModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;

            var employeesList = _dbContext.emp_info.ToList();
            model.EmpList = employeesList;

            var json = JsonConvert.SerializeObject(model.EmpList);
            model.EmpListJson = json;

            return PartialView("~/Views/AdminDashboard/EmpManagement.cshtml", model);
        }

        [HttpGet]
        public ActionResult AddEmployee(string empid)
        {
            var model = new AddEmployeeViewModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;

            model.HeadLine = "Add Employee";
            if(!string.IsNullOrEmpty(empid))
            {
                model.HeadLine = "Edit Employee";
                var ediatbleEmp = _dbContext.emp_info.Where(x => x.EmployeeID == empid).FirstOrDefault();
                if (ediatbleEmp != null)
                {
                    model.EditableEmpInfo = ediatbleEmp;
                }
            }

            //var employeesList = _dbContext.emp_info.ToList();
            //model.EmpList = employeesList;

            //var json = JsonConvert.SerializeObject(model.EmpList);
            //model.EmpListJson = json;

            return PartialView("~/Views/AdminDashboard/AddEmpTabs.cshtml", model);
        }

        [HttpPost]
        public ActionResult AddEmployee(emp_info formData)
        {
            var model = new AddEmployeeViewModel();
            try
            {
                model.EmpInfo = formData;

                _dbContext.emp_info.Add(formData);
                _dbContext.SaveChanges();

                model.JsonResponse.Message = "Employee details added successfully!";
                model.JsonResponse.StatusCode = 200;
            }
            catch (Exception ex)
            {
                model.JsonResponse = ErrorHelper.CaptureError(ex);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult ImportUser()
        {
            var model = new EmployeeManagementViewModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;

            var employeesList = _dbContext.emp_info.ToList();
            model.EmpList = employeesList;

            var json = JsonConvert.SerializeObject(model.EmpList);
            model.EmpListJson = json;

            return PartialView("~/Views/AdminDashboard/ImportUser.cshtml", model);
        }

        [HttpPost]
        public ActionResult ImportUsers(HttpPostedFileBase file)
        {
            var model = new ImportEmployeeViewModel();
            try
            {
                // Check if a file is uploaded
                if (file != null && file.ContentLength > 0)
                {
                    // Create a list to hold emp_info objects
                    List<emp_info> empList = new List<emp_info>();

                    // Load the Excel package
                    using (ExcelPackage package = new ExcelPackage(file.InputStream))
                    {
                        // Get the first worksheet in the workbook
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                        // Create a DataTable to hold the data
                        DataTable dt = new DataTable();

                        // Read the header row
                        int colCount = worksheet.Dimension.End.Column;
                        for (int col = 1; col <= colCount; col++)
                        {
                            dt.Columns.Add(worksheet.Cells[1, col].Value.ToString());
                        }

                        // Read the data rows
                        int rowCount = worksheet.Dimension.End.Row;
                        for (int row = 2; row <= rowCount; row++)
                        {
                            DataRow dataRow = dt.NewRow();
                            for (int col = 1; col <= colCount; col++)
                            {
                                dataRow[col - 1] = worksheet.Cells[row, col].Value;
                            }
                            dt.Rows.Add(dataRow);

                            // Create an emp_info object and populate its properties from DataRow
                            emp_info emp = new emp_info();

                            emp.EmployeeID = dataRow["EmployeeID"].ToString();
                            emp.EmployeeStatus = dataRow["EmployeeStatus"].ToString();
                            emp.EmployeeName = dataRow["EmployeeName"].ToString();
                            emp.FatherName = dataRow["FatherName"].ToString();
                            emp.Gender = dataRow["Gender"].ToString();
                            emp.BloodGroup = dataRow["BloodGroup"].ToString();
                            emp.DOB = Convert.ToDateTime(dataRow["DOB"]);
                            emp.DOJ = Convert.ToDateTime(dataRow["DOJ"]);
                            emp.Age = dataRow["Age"].ToString();
                            emp.MaritalStatus = dataRow["MaritalStatus"].ToString();
                            emp.Designation = dataRow["Designation"].ToString();
                            emp.ShiftTimings = dataRow["ShiftTimings"].ToString();
                            emp.Department = dataRow["Department"].ToString();
                            emp.Client = dataRow["Client"].ToString();
                            emp.EmployeeType = dataRow["EmployeeType"].ToString();
                            emp.Location = dataRow["Location"].ToString();
                            emp.ReportingManager = dataRow["ReportingManager"].ToString();
                            emp.LeavereportingManager = dataRow["LeavereportingManager"].ToString();
                            emp.MobileNumber = dataRow["MobileNumber"].ToString();
                            emp.Personal_Emailid = dataRow["Personal_Emailid"].ToString();
                            emp.OfficalEmailid = dataRow["OfficalEmailid"].ToString();
                            emp.PresentAddress = dataRow["PresentAddress"].ToString();
                            emp.Permanent_Address = dataRow["Permanent_Address"].ToString();
                            emp.RelationName = dataRow["RelationName"].ToString();
                            emp.Relationship = dataRow["Relationship"].ToString();
                            emp.FamilyMobileNumber = dataRow["FamilyMobileNumber"].ToString();
                            emp.BankName = dataRow["BankName"].ToString();
                            emp.AccountNumber = dataRow["AccountNumber"].ToString();
                            emp.Branch = dataRow["Branch"].ToString();
                            emp.TypeofAccount = dataRow["TypeofAccount"].ToString();
                            emp.IFSCcode = dataRow["IFSCcode"].ToString();
                            emp.PANnumber = dataRow["PANnumber"].ToString();
                            emp.AadharNumber = dataRow["AadharNumber"].ToString();
                            emp.Passport = dataRow["Passport"].ToString();
                            emp.DOEofPassport = !string.IsNullOrWhiteSpace(dataRow["DOEofPassport"].ToString()) ? Convert.ToDateTime(dataRow["DOEofPassport"]) : System.DateTime.MinValue;
                            emp.UANNumber = dataRow["UANNumber"].ToString();
                            emp.PFNumber = dataRow["PFNumber"].ToString();
                            emp.ESICNumber = dataRow["ESICNumber"].ToString();
                            emp.LatestDegree = dataRow["LatestDegree"].ToString();
                            emp.College_Name = dataRow["College_Name"].ToString();
                            emp.Specialization = dataRow["Specialization"].ToString();
                            emp.YearofCompletion = !string.IsNullOrWhiteSpace(dataRow["YearofCompletion"].ToString()) ? Convert.ToInt32(dataRow["YearofCompletion"]) : 0;
                            emp.Employer_name = dataRow["Employer_name"].ToString();
                            emp.JobTitle = dataRow["JobTitle"].ToString();
                            emp.From_date = !string.IsNullOrWhiteSpace(dataRow["From_date"].ToString()) ? Convert.ToDateTime(dataRow["From_date"]) : System.DateTime.MinValue;
                            emp.To_date = !string.IsNullOrWhiteSpace(dataRow["To_date"].ToString()) ? Convert.ToDateTime(dataRow["To_date"]) : System.DateTime.MinValue;
                            emp.ReasonforRelieving = dataRow["ReasonforRelieving"].ToString();
                            emp.DateofExit = !string.IsNullOrWhiteSpace(dataRow["DateofExit"].ToString()) ? Convert.ToDateTime(dataRow["DateofExit"]) : System.DateTime.MinValue;
                            emp.Reason = dataRow["Reason"].ToString();
                            emp.EligibleforRehire = dataRow["EligibleforRehire"].ToString();

                            // Add the emp_info object to the list
                            empList.Add(emp);
                        }
                    }

                    // Save the data to the database
                    if (empList.Count > 0)
                    {
                        _dbContext.emp_info.AddRange(empList);
                        _dbContext.SaveChanges();

                        model.JsonResponse.Message = "Users Imported successfully";
                        model.JsonResponse.StatusCode = 200;
                    }
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                model.JsonResponse = ErrorHelper.CaptureError(ex);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult ExportToExcel(List<string> selectedEmployeeIds)
        {
            // Retrieve selected employee data from the database
            var selectedEmployees = _dbContext.emp_info.Where(e => selectedEmployeeIds.Contains(e.EmployeeID)).ToList();

            // Create Excel package
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("EmployeeInfo");

                // Write column headers
                var properties = typeof(emp_info).GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }

                // Write data to Excel
                int row = 2;
                foreach (var employee in selectedEmployees)
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        worksheet.Cells[row, i + 1].Value = properties[i].GetValue(employee);
                    }
                    row++;
                }

                // Save Excel package to a memory stream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    excelPackage.SaveAs(memoryStream);
                    byte[] fileContents = memoryStream.ToArray();
                    return Json(Convert.ToBase64String(fileContents));
                }
            }
        }


        //public ActionResult ExportToExcel()
        //{
        //    // Retrieve employee data from the database
        //    var employees = _dbContext.emp_info.ToList();

        //    // Create Excel package
        //    using (ExcelPackage excelPackage = new ExcelPackage())
        //    {
        //        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("EmployeeInfo");

        //        // Write column headers
        //        var properties = typeof(emp_info).GetProperties();
        //        for (int i = 0; i < properties.Length; i++)
        //        {
        //            worksheet.Cells[1, i + 1].Value = properties[i].Name;
        //        }

        //        // Write data to Excel
        //        int row = 2;
        //        foreach (var employee in employees)
        //        {
        //            for (int i = 0; i < properties.Length; i++)
        //            {
        //                worksheet.Cells[row, i + 1].Value = properties[i].GetValue(employee);
        //            }
        //            row++;
        //        }

        //        // Save Excel package
        //        byte[] fileContents = excelPackage.GetAsByteArray();
        //        return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeInfo.xlsx");

        //    }
        //}

        public JsonResult DeleteEmp(DeleteEmployeeViewModel deletEmpInfo)
        {
            var deleteModel = new DeleteEmployeeViewModel();
            try
            {
                deleteModel.empID = deletEmpInfo.empID;
                var employee = _dbContext.emp_info.Where(emp => emp.EmployeeID == deletEmpInfo.empID).FirstOrDefault();

                if (employee == null)
                {
                    deleteModel.JsonResponse.Message = "Employee deleted successfully!";
                    deleteModel.JsonResponse.StatusCode = 200;
                    return Json(deleteModel, JsonRequestBehavior.AllowGet);
                }

                _dbContext.emp_info.Remove(employee);
                _dbContext.SaveChanges();

                deleteModel.JsonResponse.Message = deletEmpInfo.empName + " Employee deleted successfully!";
                deleteModel.JsonResponse.StatusCode = 200;
            }
            catch (Exception ex)
            {
                deleteModel.JsonResponse = ErrorHelper.CaptureError(ex);
            }
            return Json(deleteModel, JsonRequestBehavior.AllowGet);
        }


        //[CustomAuthorize(Roles = "HR Admin")]
        public ActionResult AttendanceManagement()
        {
            //ViewBag.ActivePage = "AttendanceManagement";
            return PartialView("~/Views/AdminDashboard/AddEmpTabs.cshtml");
        }

        //[CustomAuthorize(Roles = "HR Admin")]
        public ActionResult LeaveManagement()
        {
            ViewBag.ActivePage = "LeaveManagement";
            return View();
        }

        //[CustomAuthorize(Roles = "HR Admin")]
        public ActionResult Ticketing()
        {
            ViewBag.ActivePage = "Ticketing";
            return View();
        }



        //// POST: Designation/Add
        //[HttpPost]
        //public ActionResult DesignationAdd(string newDesignation)
        //{

        //    if (!string.IsNullOrEmpty(newDesignation))
        //    {
        //        try
        //        {
        //            // Create a new Designation object
        //            var designation = new Designation { Name = newDesignation };

        //            // Add the new designation to the context and save changes
        //            _dbContext.Designations.Add(designation);
        //            _dbContext.SaveChanges();

        //            // Return a success message
        //            return Json(new { success = true });
        //        }
        //        catch (Exception ex)
        //        {
        //            // Log the error
        //            Console.WriteLine($"Error adding new designation: {ex.Message}");

        //            // Return an error message
        //            return Json(new { error = "An error occurred while adding the designation." });
        //        }
        //    }

        //    // Return a bad request if the input is empty
        //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //}
    }
}
