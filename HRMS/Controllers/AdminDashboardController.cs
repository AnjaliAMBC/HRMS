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

using System.Net.Mail;
using System.Configuration;
using System.Net;
using HRMS.Models.Employee;

namespace HRMS.Controllers
{
    public class AdminDashboardController : BaseController
    {
        // Database context
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public AdminDashboardController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }

        // GET: AdminDashboard
        [CustomAuthorize(Roles = "HR Admin, Super Admin, IT Admin")]
        public ActionResult Dashboard()
        {
            var model = new AdminDashView();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;
            return View("~/Views/AdminDashboard/Dashboard.cshtml", model);
        }

        //[CustomAuthorize(Roles = "HR Admin")]
        [CustomAuthorize(Roles = "HR Admin, Super Admin")]
        public ActionResult EmployeeManagement()
        {
            var model = new EmployeeManagementViewModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;

            var employeesList = _dbContext.emp_info.OrderByDescending(emp => emp.EmployeeID).ToList();
            model.EmpList = employeesList;

            var json = JsonConvert.SerializeObject(model.EmpList);
            model.EmpListJson = json;

            model.Departments = new EmployeeHelper(_dbContext).GetDepartments();

            return View("~/Views/AdminDashboard/EmpManagement.cshtml", model);
        }
        public ActionResult AdminJobListing()
        {
            return View("~/Views/AdminDashboard/AdminJobListing.cshtml");
        }
        public ActionResult AdminJobDetail()
        {
            return View("~/Views/AdminDashboard/AdminJobDetail.cshtml");
        }

        [HttpGet]
        public ActionResult AddEmployee(string empid)
        {
            var model = new AddEmployeeViewModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;

            model.HeadLine = "Add Employee";
            if (!string.IsNullOrEmpty(empid) && !model.IsAddAction)
            {
                model.HeadLine = "Edit Employee";
                var ediatbleEmp = _dbContext.emp_info.Where(x => x.EmployeeID == empid).FirstOrDefault();
                if (ediatbleEmp != null)
                {
                    model.EditableEmpInfo = ediatbleEmp;
                }
            }
            else
            {
                model.IsAddAction = true;
            }

            model.Clients = new EmployeeHelper(_dbContext).GetClients();
            model.Designations = new EmployeeHelper(_dbContext).GetDesignations();
            model.Departments = new EmployeeHelper(_dbContext).GetDepartments();
            model.LeaveManagers = new EmployeeHelper(_dbContext).GetLeaveManagers();
            model.ReportingManagers = new EmployeeHelper(_dbContext).GetReportingManagers();

            var lastEmpInfo = _dbContext.emp_info.OrderByDescending(e => e.EmployeeID).FirstOrDefault();
            if (lastEmpInfo != null)
            {
                model.LastCreatedEmpID = lastEmpInfo.EmployeeID;
            }


            return PartialView("~/Views/AdminDashboard/AddEmpTabs.cshtml", model);
        }

        [HttpPost]
        public ActionResult AddEmployee(emp_info formData)
        {
            var model = new AddEmployeeViewModel();
            try
            {
                model.EmpInfo = formData;

                var newEmployee = _dbContext.emp_info.Add(formData);
                _dbContext.SaveChanges();

                model.JsonResponse.Message = "Employee details added successfully!";
                model.JsonResponse.StatusCode = 200;

                var empLoginInfo = new emplogin();
                empLoginInfo.EmployeeID = formData.EmployeeID;
                empLoginInfo.EmployeeName = formData.EmployeeName;
                empLoginInfo.EmployeeMobile = formData.MobileNumber;
                empLoginInfo.Password = PasswordHelper.GenerateRandomPassword(10);
                empLoginInfo.IsLeaveRM = false;
                empLoginInfo.IsReportingM = false;
                empLoginInfo.EmployeeStatus = "Active";
                empLoginInfo.EmployeeRole = "Team Member";
                empLoginInfo.EmployeeEmail = formData.OfficalEmailid;
                empLoginInfo.CreatedBy = formData.CreatedBy;
                empLoginInfo.CreatedDate = DateTime.Now;

                _dbContext.emplogins.Add(empLoginInfo);
                _dbContext.SaveChanges();


                //Add leave balnce in case employee added as a full time employee without any probation
                if (model.EmpInfo.EmployeeType == "Full-Time")
                {
                    var currentYear = DateTime.Now.Year.ToString();
                    var empAvailableLeave = new AvailableLeaves();
                    var empLeaveBalance = new LeaveBalance();

                    var leaveTypes = new LeaveCalculator().LeaveCategories();

                    var empLeaveBalanceInfo = _dbContext.LeaveBalances.Where(x => x.Year == currentYear && x.EmpID == formData.EmployeeID).FirstOrDefault();

                    foreach (var leaveType in leaveTypes)
                    {
                        empAvailableLeave = new LeaveCalculator().CalculateAvailableLeaves(newEmployee, leaveType, true);

                        if (leaveType.Type == "Earned Leave")
                        {
                            empLeaveBalance.Earned = empAvailableLeave.Available;
                        }
                        if (leaveType.Type == "Emergency Leave")
                        {
                            empLeaveBalance.Emergency = empAvailableLeave.Available;
                        }
                        if (leaveType.Type == "Sick Leave")
                        {
                            empLeaveBalance.Sick = empAvailableLeave.Available;
                        }
                        if (leaveType.Type == "Bereavement Leave")
                        {
                            empLeaveBalance.Bereavement = empAvailableLeave.Available;
                        }
                        if (leaveType.Type == "Hourly Permission")
                        {
                            empLeaveBalance.HourlyPermission = empAvailableLeave.Available;
                        }
                        if (leaveType.Type == "Marriage Leave")
                        {
                            empLeaveBalance.Marriage = empAvailableLeave.Available;
                        }
                        if (leaveType.Type == "Maternity Leave")
                        {
                            empLeaveBalance.Maternity = empAvailableLeave.Available;
                        }
                        if (leaveType.Type == "Paternity Leave")
                        {
                            empLeaveBalance.Paternity = empAvailableLeave.Available;
                        }
                        if (leaveType.Type == "Comp Off")
                        {
                            empLeaveBalance.CompOff = empAvailableLeave.Available;
                        }
                    }
                    empLeaveBalance.EmpID = newEmployee.EmployeeID;
                    empLeaveBalance.EmployeeName = newEmployee.EmployeeName;
                    empLeaveBalance.Year = currentYear;

                    //Add new entry
                    _dbContext.LeaveBalances.Add(empLeaveBalance);

                    //Delete old entry in case
                    if(empLeaveBalanceInfo != null)
                    {
                        _dbContext.LeaveBalances.Remove(empLeaveBalanceInfo);
                    }                  
                    _dbContext.SaveChanges();


                }

                NewAccountCreateEmail(empLoginInfo);
            }
            catch (Exception ex)
            {
                model.JsonResponse = ErrorHelper.CaptureError(ex);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult NewAccountCreateEmail(emplogin empInfoModel)
        {
            try
            {
                var emailBody = "";
                var emailSubject = "Welcome to AMBC! Access Your AMBC Employee Portal";
                emailBody = PartialViewHelper.RenderPartialToString(this, "_newLoginpassword", empInfoModel, ViewData, TempData);

                using (MailMessage mm = new MailMessage(ConfigurationManager.AppSettings["SMTPUserName"], empInfoModel.EmployeeEmail))
                {
                    mm.Subject = emailSubject;
                    mm.Body = emailBody;
                    mm.IsBodyHtml = true;

                    mm.CC.Add(ConfigurationManager.AppSettings["LoginNotificationCC"]);

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = ConfigurationManager.AppSettings["SMTPHost"];
                    smtp.EnableSsl = true;
                    NetworkCredential credentials = new NetworkCredential();
                    credentials.UserName = ConfigurationManager.AppSettings["SMTPUserName"];
                    credentials.Password = ConfigurationManager.AppSettings["SMTPPassword"];
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = credentials;
                    smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                    smtp.Send(mm);
                }
                return Json(null);
            }
            catch (Exception ex)
            {
                ErrorHelper.CaptureError(ex);
                return Json("Error occured while sending new emp login info email: ", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UpdateEmployee(emp_info formData)
        {
            var model = new AddEmployeeViewModel();
            try
            {
                model.EmpInfo = formData;
                var existingEmployee = _dbContext.emp_info.Where(emp => emp.EmployeeID == formData.EmployeeID).FirstOrDefault();

                var earlierEmploymentType = existingEmployee.EmployeeType;
                if (existingEmployee != null)
                {
                    //In here in case of status chnage updating in emp login table as well
                    if (existingEmployee.EmployeeStatus != model.EmpInfo.EmployeeStatus)
                    {
                        var loginRelatedtoEmp = _dbContext.emplogins.Where(x => x.EmployeeID == existingEmployee.EmployeeID).FirstOrDefault();
                        if (loginRelatedtoEmp != null)
                        {
                            loginRelatedtoEmp.EmployeeStatus = model.EmpInfo.EmployeeStatus;
                            _dbContext.SaveChanges();
                        }
                    }

                    // Update all properties of the existing entity
                    existingEmployee.EmployeeStatus = model.EmpInfo.EmployeeStatus;
                    existingEmployee.EmployeeName = model.EmpInfo.EmployeeName;
                    existingEmployee.FatherName = model.EmpInfo.FatherName;
                    existingEmployee.Gender = model.EmpInfo.Gender;
                    existingEmployee.BloodGroup = model.EmpInfo.BloodGroup;
                    existingEmployee.DOB = model.EmpInfo.DOB;
                    existingEmployee.DOJ = model.EmpInfo.DOJ;
                    existingEmployee.Age = model.EmpInfo.Age;
                    existingEmployee.MaritalStatus = model.EmpInfo.MaritalStatus;
                    existingEmployee.Designation = model.EmpInfo.Designation;
                    existingEmployee.ShiftTimings = model.EmpInfo.ShiftTimings;
                    existingEmployee.Department = model.EmpInfo.Department;
                    existingEmployee.Client = model.EmpInfo.Client;
                    existingEmployee.EmployeeType = model.EmpInfo.EmployeeType;
                    existingEmployee.Location = model.EmpInfo.Location;
                    existingEmployee.ReportingManager = model.EmpInfo.ReportingManager;
                    existingEmployee.LeavereportingManager = model.EmpInfo.LeavereportingManager;
                    existingEmployee.MobileNumber = model.EmpInfo.MobileNumber;
                    existingEmployee.Personal_Emailid = model.EmpInfo.Personal_Emailid;
                    existingEmployee.OfficalEmailid = model.EmpInfo.OfficalEmailid;
                    existingEmployee.PresentAddress = model.EmpInfo.PresentAddress;
                    existingEmployee.Permanent_Address = model.EmpInfo.Permanent_Address;
                    existingEmployee.RelationName = model.EmpInfo.RelationName;
                    existingEmployee.Relationship = model.EmpInfo.Relationship;
                    existingEmployee.FamilyMobileNumber = model.EmpInfo.FamilyMobileNumber;
                    existingEmployee.BankName = model.EmpInfo.BankName;
                    existingEmployee.AccountNumber = model.EmpInfo.AccountNumber;
                    existingEmployee.Branch = model.EmpInfo.Branch;
                    existingEmployee.TypeofAccount = model.EmpInfo.TypeofAccount;
                    existingEmployee.IFSCcode = model.EmpInfo.IFSCcode;
                    existingEmployee.PANnumber = model.EmpInfo.PANnumber;
                    existingEmployee.AadharNumber = model.EmpInfo.AadharNumber;
                    existingEmployee.Passport = model.EmpInfo.Passport;
                    existingEmployee.DOEofPassport = model.EmpInfo.DOEofPassport;
                    existingEmployee.UANNumber = model.EmpInfo.UANNumber;
                    existingEmployee.PFNumber = model.EmpInfo.PFNumber;
                    existingEmployee.ESICNumber = model.EmpInfo.ESICNumber;
                    existingEmployee.LatestDegree = model.EmpInfo.LatestDegree;
                    existingEmployee.College_Name = model.EmpInfo.College_Name;
                    existingEmployee.Specialization = model.EmpInfo.Specialization;
                    existingEmployee.YearofCompletion = model.EmpInfo.YearofCompletion;
                    existingEmployee.Employer_name = model.EmpInfo.Employer_name;
                    existingEmployee.JobTitle = model.EmpInfo.JobTitle;
                    existingEmployee.From_date = model.EmpInfo.From_date;
                    existingEmployee.To_date = model.EmpInfo.To_date;
                    existingEmployee.ReasonforRelieving = model.EmpInfo.ReasonforRelieving;
                    existingEmployee.DateofExit = model.EmpInfo.DateofExit;
                    existingEmployee.Reason = model.EmpInfo.Reason;
                    existingEmployee.EligibleforRehire = model.EmpInfo.EligibleforRehire;
                    existingEmployee.CreatedBy = model.EmpInfo.CreatedBy;
                    existingEmployee.CreatedDate = model.EmpInfo.CreatedDate;
                    existingEmployee.UpdatedBy = model.EmpInfo.UpdatedBy;
                    existingEmployee.UpdatedDate = DateTime.Now;

                    _dbContext.SaveChanges();
                    model.JsonResponse.Message = "Employee details updated successfully!";
                    model.JsonResponse.StatusCode = 200;

                    if (earlierEmploymentType == "Probation" && model.EmpInfo.EmployeeType == "Full-Time")
                    {
                        var currentYear = DateTime.Now.Year.ToString();
                        var empAvailableLeave = new AvailableLeaves();
                        var empLeaveBalance = new LeaveBalance();

                        var currentYearFirstDate = new DateTime(DateTime.Now.Year, 1, 1);
                        var lastYearFirstDate = currentYearFirstDate.AddMonths(-12);
                        var leaveTypes = new LeaveCalculator().LeaveCategories();

                        var empLeaveBalanceInfo = _dbContext.LeaveBalances.Where(x => x.Year == currentYear && x.EmpID == existingEmployee.EmployeeID).FirstOrDefault();

                        foreach (var leaveType in leaveTypes)
                        {
                            empAvailableLeave = new LeaveCalculator().CalculateAvailableLeaves(existingEmployee, leaveType);

                            if (leaveType.Type == "Earned Leave")
                            {
                                empLeaveBalance.Earned = empAvailableLeave.Available;
                            }
                            if (leaveType.Type == "Emergency Leave")
                            {
                                empLeaveBalance.Emergency = empAvailableLeave.Available;
                            }
                            if (leaveType.Type == "Sick Leave")
                            {
                                empLeaveBalance.Sick = empAvailableLeave.Available;
                            }
                            if (leaveType.Type == "Bereavement Leave")
                            {
                                empLeaveBalance.Bereavement = empAvailableLeave.Available;
                            }
                            if (leaveType.Type == "Hourly Permission")
                            {
                                empLeaveBalance.HourlyPermission = empAvailableLeave.Available;
                            }
                            if (leaveType.Type == "Marriage Leave")
                            {
                                empLeaveBalance.Marriage = empAvailableLeave.Available;
                            }
                            if (leaveType.Type == "Maternity Leave")
                            {
                                empLeaveBalance.Maternity = empAvailableLeave.Available;
                            }
                            if (leaveType.Type == "Paternity Leave")
                            {
                                empLeaveBalance.Paternity = empAvailableLeave.Available;
                            }
                            if (leaveType.Type == "Comp Off")
                            {
                                empLeaveBalance.CompOff = empAvailableLeave.Available;
                            }
                        }
                        empLeaveBalance.EmpID = existingEmployee.EmployeeID;
                        empLeaveBalance.EmployeeName = existingEmployee.EmployeeName;
                        empLeaveBalance.Year = currentYear;

                        //Add new entry
                        _dbContext.LeaveBalances.Add(empLeaveBalance);
                        //Delete old entry
                        _dbContext.LeaveBalances.Remove(empLeaveBalanceInfo);
                        _dbContext.SaveChanges();


                    }
                }
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
            var cuserContext = SiteContext.GetCurrentUserContext();

            try
            {
                // Check if a file is uploaded
                if (file != null && file.ContentLength > 0)
                {
                    List<emp_info> empList = new List<emp_info>();

                    using (ExcelPackage package = new ExcelPackage(file.InputStream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                        DataTable dt = new DataTable();

                        int colCount = worksheet.Dimension.End.Column;
                        for (int col = 1; col <= colCount; col++)
                        {
                            dt.Columns.Add(worksheet.Cells[1, col].Value.ToString());
                        }

                        int rowCount = worksheet.Dimension.End.Row;
                        for (int row = 2; row <= rowCount; row++)
                        {
                            bool isEmptyRow = true;
                            for (int col = 1; col <= colCount; col++)
                            {
                                if (worksheet.Cells[row, col].Value != null)
                                {
                                    isEmptyRow = false;
                                    break;
                                }
                            }

                            if (isEmptyRow)
                            {
                                continue;
                            }

                            DataRow dataRow = dt.NewRow();
                            for (int col = 1; col <= colCount; col++)
                            {
                                dataRow[col - 1] = worksheet.Cells[row, col].Value;
                            }

                            dt.Rows.Add(dataRow);
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
                            emp.CreatedBy = cuserContext.LoginInfo.EmployeeName;
                            emp.CreatedDate = DateTime.Now;
                            empList.Add(emp);
                        }
                    }

                    if (empList.Count > 0)
                    {
                        _dbContext.emp_info.AddRange(empList);
                        _dbContext.SaveChanges();

                        model.JsonResponse.Message = "Users Imported successfully";
                        model.JsonResponse.StatusCode = 200;

                        foreach (var importEmp in empList)
                        {
                            var empLoginInfo = new emplogin();
                            empLoginInfo.EmployeeID = importEmp.EmployeeID;
                            empLoginInfo.EmployeeName = importEmp.EmployeeName;
                            empLoginInfo.EmployeeMobile = importEmp.MobileNumber;
                            empLoginInfo.Password = PasswordHelper.GenerateRandomPassword(10);
                            empLoginInfo.IsLeaveRM = false;
                            empLoginInfo.IsReportingM = false;
                            empLoginInfo.EmployeeStatus = "Active";
                            empLoginInfo.EmployeeRole = "Team Member";
                            empLoginInfo.EmployeeEmail = importEmp.OfficalEmailid;
                            empLoginInfo.CreatedBy = importEmp.CreatedBy;
                            empLoginInfo.CreatedDate = DateTime.Now;

                            _dbContext.emplogins.Add(empLoginInfo);
                            _dbContext.SaveChanges();

                            NewAccountCreateEmail(empLoginInfo);
                        }
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
            var selectedEmployees = _dbContext.emp_info.Where(e => selectedEmployeeIds.Contains(e.EmployeeID)).ToList();

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("EmployeeInfo");

                var properties = typeof(emp_info).GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }

                using (var firstRow = worksheet.Cells[1, 1, 1, properties.Length])
                {
                    firstRow.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    firstRow.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SkyBlue);
                    firstRow.Style.Font.Color.SetColor(System.Drawing.Color.White); // Optional: Set text color to white for contrast
                }
                int row = 2;
                foreach (var employee in selectedEmployees)
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var value = properties[i].GetValue(employee);
                        if (value is DateTime)
                        {
                            worksheet.Cells[row, i + 1].Style.Numberformat.Format = "yyyy-mm-dd";
                            worksheet.Cells[row, i + 1].Value = ((DateTime)value).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            worksheet.Cells[row, i + 1].Value = value;
                        }
                    }
                    row++;
                }

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    excelPackage.SaveAs(memoryStream);
                    byte[] fileContents = memoryStream.ToArray();
                    return Json(Convert.ToBase64String(fileContents));
                }
            }
        }


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

                var loginEmployee = _dbContext.emplogins.Where(emp => emp.EmployeeID == deletEmpInfo.empID).FirstOrDefault();
                if (loginEmployee != null)
                {
                    _dbContext.emplogins.Remove(loginEmployee);
                    _dbContext.SaveChanges();
                }

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


        [HttpPost]
        public ActionResult DesignationAdd(string newDesignation)
        {
            var model = new JsonResponse();
            if (!string.IsNullOrEmpty(newDesignation))
            {
                try
                {
                    // Create a new Designation object
                    var designation = new Designation { Designation1 = newDesignation };

                    // Add the new designation to the context and save changes
                    _dbContext.Designations.Add(designation);
                    _dbContext.SaveChanges();

                    model.Message = "Designation added successfully!";
                    model.StatusCode = 200;

                    // Return a success message
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    model = ErrorHelper.CaptureError(ex);
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DepartmentAdd(string newDepartment)
        {
            var model = new JsonResponse();
            if (!string.IsNullOrEmpty(newDepartment))
            {
                try
                {
                    var department = new Department { DepartmentName = newDepartment };
                    _dbContext.Departments.Add(department);
                    _dbContext.SaveChanges();

                    model.Message = "Department added successfully!";
                    model.StatusCode = 200;

                    // Return a success message
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    model = ErrorHelper.CaptureError(ex);
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ClientAdd(string newClient)
        {
            var model = new JsonResponse();
            if (!string.IsNullOrEmpty(newClient))
            {
                try
                {
                    // Create a new Designation object
                    var client = new Client { ClientName = newClient };

                    // Add the new designation to the context and save changes
                    _dbContext.Clients.Add(client);
                    _dbContext.SaveChanges();

                    model.Message = "Client added successfully!";
                    model.StatusCode = 200;

                    // Return a success message
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    model = ErrorHelper.CaptureError(ex);

                    // Return a success message
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }

            // Return a bad request if the input is empty
            return Json(model, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult RMadd(string newRM)
        {
            var model = new JsonResponse();
            if (!string.IsNullOrEmpty(newRM))
            {
                try
                {
                    var newReportingManager = _dbContext.LeaveRMs.Where(emp => emp.Name == newRM).FirstOrDefault();
                    if (newReportingManager == null)
                    {
                        var newReprotingM = new LeaveRM { Name = newRM, IsReporingM = true };
                        _dbContext.LeaveRMs.Add(newReprotingM);
                        _dbContext.SaveChanges();

                        model.Message = "Reporting Manager added successfully!";
                        model.StatusCode = 200;
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (newReportingManager != null)
                        {
                            newReportingManager.IsReporingM = true;
                            _dbContext.SaveChanges();

                            model.Message = "Reporting Manager added successfully!";
                            model.StatusCode = 200;
                            return Json(model, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            model.Message = "Reporting Manager not added!";
                            model.StatusCode = 400;
                            return Json(model, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                catch (Exception ex)
                {
                    model = ErrorHelper.CaptureError(ex);
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LMadd(string newLM)
        {
            var model = new JsonResponse();
            if (!string.IsNullOrEmpty(newLM))
            {
                try
                {
                    var newLeaveManager = _dbContext.LeaveRMs.Where(emp => emp.Name == newLM).FirstOrDefault();
                    if (newLeaveManager == null)
                    {
                        var newReprotingM = new LeaveRM { Name = newLM, IsLeaveRM = true };
                        _dbContext.LeaveRMs.Add(newReprotingM);
                        _dbContext.SaveChanges();

                        model.Message = "Leave Manager added successfully!";
                        model.StatusCode = 200;
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        model.Message = "Leave Manager not added!";
                        model.StatusCode = 400;
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    model = ErrorHelper.CaptureError(ex);
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
       
    }
}
