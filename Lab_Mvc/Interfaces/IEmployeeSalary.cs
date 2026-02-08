using Models;

namespace Lab_Mvc.Interfaces
{
    public interface IEmployeeSalary
    {
        Task<IEnumerable<DTOEmployeeSalary>> GetEmployeeSalary(int comId);
        Task<DTOEmployeeSalary> GetEmployeeSalaryById(long empSal_id, int comId);
        Task<List<DTOEmployeeSalary>> GetDateWiseEmpSalary(string from_date, string to_date, int comId);
        Task SaveEmployeeSalary(DTOEmployeeSalary objEmpSlry);
        Task EditEmployeeSalary(DTOEmployeeSalary objEmpSlry, long empSal_id);
        Task DeleteEmployeeSalary(long empSal_id, int comId);
    }
}
