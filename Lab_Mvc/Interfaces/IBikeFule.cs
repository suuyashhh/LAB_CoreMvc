using Models;

namespace Lab_Mvc.Interfaces
{
    public interface IBikeFule
    {
        Task<IEnumerable<DTOBikeFule>> GetBikeFule();
        Task<IEnumerable<DTOBikeFule>> GetBikeFuleById(long bike_id);
        Task SaveBikeFule(DTOBikeFule objBike);
        Task EditBikeFule(DTOBikeFule objBike, long bike_id);
        Task DeleteBikeFule(long bike_id);
    }
}
