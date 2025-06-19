namespace Lab_Mvc.Constants
{
    public class QueryConstant
    {
        public static string Login = "Login";

        public static string GetTests = "select * from MST_TEST";
        public static string GetTestById = "GetTestById"; 
        public static string InsertTest = "InsertTest";
        public static string UpdateTest = "UpdateTest";
        public static string DeleteTest = "DeleteTest"; 


        public static string GetDoctors = "select * from MST_DOCTOR";
        public static string GetDoctorById = "GetDoctorById";
        public static string InsertDoctor = "InsertDoctor";
        public static string UpdateDoctor = "UpdateDoctor";
        public static string DeleteDoctor = "DeleteDoctor";


        public static string GetCasePapers = "select * from MST_PATIENT order by trN_NO desc";
        public static string GetCasePaperById = "GetCasePaperById";
        public static string InsertCasePaper = "InsertCasePaper";
        public static string UpdateCasePaper = "UpdateCasePaper";
        public static string DeleteCasePaper = "DeleteCasePaper";

        public static string GetEmployees = "select * from MST_EMPLOYEE";
        public static string GetEmployeeById = "GetEmployeeById";
        public static string InsertEmployee = "InsertEmployee";
        public static string UpdateEmployee = "UpdateEmployee";
        public static string DeleteEmployee = "DeleteEmployee";

        public static string GetLabMaterials = "Select * from MST_MATERIALS ";
        public static string GetMaterialById = "GetMaterialById";
        public static string InsertMaterials = "InsertMaterials";
        public static string UpdateMaterials = "UpdateMaterials";
        public static string DeleteMaterials = "DeleteMaterials";

        public static string GetBikeFule = "Select * from MST_BIKE_FULE ";
        public static string GetBikeFuleById = "GetBikeFuleById";
        public static string InsertBikeFule = "InsertBikeFule";
        public static string UpdateBikeFule = "UpdateBikeFule";
        public static string DeleteBikeFule = "DeleteBikeFule";

        public static string GetEmployeeSalary = "Select * from MST_EMP_SALARY_SLIP ";
        public static string GetEmployeeSalaryById = "GetEmployeeSalaryById";
        public static string InsertEmployeeSalary = "InsertEmployeeSalary";
        public static string UpdateEmployeeSalary = "UpdateEmployeeSalary";
        public static string DeleteEmployeeSalary = "DeleteEmployeeSalary";

        public static string GetElectricityBill = "Select * from MST_ELECTRICITY_BILL ";
        public static string GetElectricityBillById = "GetElectricityBillById";
        public static string InsertElectricityBill = "InsertElectricityBill";
        public static string UpdateElectricityBill = "UpdateElectricityBill";
        public static string DeleteElectricityBill = "DeleteElectricityBill";

        public static string GetOtherExpense = "Select * from MST_OTHER_EXPANCE ";
        public static string GetOtherExpenseById = "GetOtherExpenseById";
        public static string InsertOtherExpense = "InsertOtherExpense";
        public static string UpdateOtherExpense = "UpdateOtherExpense";
        public static string DeleteOtherExpense = "DeleteOtherExpense";

        public static string GetDoctorCommission = "Select * from MST_DOCTOR_COMMISSION ";
        public static string GetDoctorCommissionById = "GetDoctorCommissionById";
        public static string InsertDoctorCommission = "InsertDoctorCommission";
        public static string UpdateDoctorCommission = "UpdateDoctorCommission";
        public static string DeleteDoctorCommission = "DeleteDoctorCommission";
    }
}
