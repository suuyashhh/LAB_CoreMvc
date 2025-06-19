using Models;

namespace Lab_Mvc.Interfaces
{
    public interface IEmployeeSalary
    {
        Task<IEnumerable<DTOEmployeeSalary>> GetEmployeeSalary();
        Task<DTOEmployeeSalary> GetEmployeeSalaryById(long empSal_id);
        Task SaveEmployeeSalary(DTOEmployeeSalary objEmpSlry);
        Task EditEmployeeSalary(DTOEmployeeSalary objEmpSlry, long empSal_id);
        Task DeleteEmployeeSalary(long empSal_id);
    }
}
