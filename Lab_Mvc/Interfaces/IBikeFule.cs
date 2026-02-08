using Models;

namespace Lab_Mvc.Interfaces
{
    public interface IBikeFule
    {
        Task<IEnumerable<DTOBikeFule>> GetBikeFule(int comId);
        Task<DTOBikeFule> GetBikeFuleById(long bike_id, int comId);
        Task<List<DTOBikeFule>> GetDateWiseBikeFule(string from_date, string to_date, int comId);
        Task SaveBikeFule(DTOBikeFule objBike);
        Task EditBikeFule(DTOBikeFule objBike, long bike_id);
        Task DeleteBikeFule(long bike_id, int comId);
    }
}
