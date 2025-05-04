namespace Lab_Mvc.Constants
{
    public class QueryConstant
    {
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


        public static string GetCasePapers = "select * from MST_PATIENT";
        public static string GetCasePaperById = "GetCasePaperById";
        public static string InsertCasePaper = "InsertCasePaper";
        public static string UpdateCasePaper = "UpdateCasePaper";
        public static string DeleteCasePaper = "DeleteCasePaper";
    }
}
