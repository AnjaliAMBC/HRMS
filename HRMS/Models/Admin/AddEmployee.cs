using System;
using System.Collections.Generic;

namespace HRMS.Models.Admin
{
    public class AddEmployee
    {
        public string employeeID { get; set; }
        public string employeeName { get; set; }
        public string fatherName { get; set; }
        public DateTime dateOfJoining { get; set; }
        public DateTime dateOfBirth { get; set; }
        public int age { get; set; }
        public string bloodGroup { get; set; }
        public string employeeStatus { get; set; }
        public string gender { get; set; }
        public string maritalStatus { get; set; }
        public string designation { get; set; }
        public string departmentSelect { get; set; }
        public string client { get; set; }
        public string employeeType { get; set; }
        public string location { get; set; }
        public string reportingManager { get; set; }
        public string leaveRM { get; set; }
        public string mobileNumber { get; set; }
        public string email { get; set; }
        public string officialEmail { get; set; }
        public string presentAddress { get; set; }
        public string permanentAddress { get; set; }
        public string emergencyContactName { get; set; }
        public string emergencyContactRelationship { get; set; }
        public string emergencyContactMobileNumber { get; set; }

        public string bankName { get; set; }
        public string accountNumber { get; set; }
        public string branch { get; set; }
        public string typeOfAccount { get; set; }
        public string ifscCode { get; set; }

        public string panNumber { get; set; }
        public string aadharNumber { get; set; }
        public string passportNumber { get; set; }
        public DateTime passportDOE { get; set; }

        public string uanNumber { get; set; }
        public string pfNumber { get; set; }
        public DateTime esicNumber { get; set; }


        public string latestDegree { get; set; }
        public string collegeName { get; set; }
        public string specialization { get; set; }
        public int yearOfCompletion { get; set; }

        public string employerName { get; set; }
        public string jobTitle { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public string reasonOfRelieving { get; set; }

        public DateTime dateOfExit { get; set; }
        public string reason { get; set; }
        public string eligibleForRehire { get; set; }
    }

    public class AddEmployeeViewModel : SiteContextModel
    {
        public emp_info EditableEmpInfo { get; set; } = new emp_info();
        public JsonResponse JsonResponse { get; set; } = new JsonResponse();
        public JsonResponse NewLoginEmailResponse { get; set; } = new JsonResponse();
        public string HeadLine { get; set; }
        public List<DropdownItem> Departments { get; set; } = new List<DropdownItem>();
        public List<DropdownItem> Designations { get; set; } = new List<DropdownItem>();
        public List<DropdownItem> Clients { get; set; } = new List<DropdownItem>();
        public List<DropdownItem> LeaveManagers { get; set; } = new List<DropdownItem>();
        public List<DropdownItem> ReportingManagers { get; set; } = new List<DropdownItem>();
        public bool IsAddAction { get; set; } = false;
        public string LastCreatedEmpID { get; set; }
    }

    public class DeleteEmployeeViewModel : SiteContextModel
    {
        public string empID { get; set; }
        public string empName { get; set; }
        public JsonResponse JsonResponse { get; set; } = new JsonResponse();
    }

    public class ImportEmployeeViewModel : SiteContextModel
    {
        public JsonResponse JsonResponse { get; set; } = new JsonResponse();
    }
}