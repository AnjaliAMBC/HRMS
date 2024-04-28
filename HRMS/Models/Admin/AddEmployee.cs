using System;

namespace HRMS.Models.Admin
{
    public class AddEmployee
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string FatherName { get; set; }
        public DateTime DateOfJoining { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string BloodGroup { get; set; }
        public string EmployeeStatus { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Client { get; set; }
        public string EmployeeType { get; set; }
        public string Location { get; set; }
        public string ReportingManager { get; set; }
        public string LeaveRM { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string OfficialEmail { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactRelationship { get; set; }
        public string EmergencyContactMobileNumber { get; set; }

        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string Branch { get; set; }
        public string TypeOfAccount { get; set; }
        public string IFSCCode { get; set; }

        public string LatestDegree { get; set; }
        public string CollegeName { get; set; }
        public string Specialization { get; set; }
        public int YearOfCompletion { get; set; }

        public string EmployerName { get; set; }
        public string JobTitle { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ReasonOfRelieving { get; set; }

        public DateTime DateOfExit { get; set; }
        public string Reason { get; set; }
        public string EligibleForRehire { get; set; }
    }

    public class GeneralInformationModel
    {

    }

    public class BankDetailsModel
    {

    }

    public class EducationDetailsModel
    {

    }

    public class PreviousEmployerDetailsModel
    {

    }
    public class SeparationInformationModel
    {

    }
}