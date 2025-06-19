using Models;

namespace Lab_Mvc.Interfaces
{
    public interface IDoctor
    {
        Task<IEnumerable<DTODoctor>> GetDoctors();
        Task<DTODoctor> GetDoctorById(long doctor_code);
        Task SaveDoctor(DTODoctor doctor);
        Task EditDoctor(DTODoctor doctor, long doctor_code);
        Task DeleteDoctor(long doctor_code);
    }
}
