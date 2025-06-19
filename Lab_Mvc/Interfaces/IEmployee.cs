using Models;

namespace Lab_Mvc.Interfaces
{
    public interface IEmployee
    {
        Task<IEnumerable<DTOEmployee>> GetEmployees();
        Task<DTOEmployee> GetEmployeeById(long emp_code);
        Task SaveEmployee(DTOEmployee emp);
        Task EditEmployee(DTOEmployee emp, long emp_code);
        Task DeleteEmployee(long emp_code);
    }
}
