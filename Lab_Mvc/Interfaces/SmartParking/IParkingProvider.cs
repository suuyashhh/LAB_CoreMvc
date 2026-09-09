using Models;

namespace SmartParking.Interfaces
{
    public interface IParkingProvider
    {
        Task<string> SaveParkingLocation(DTOParkingProvider DTOParkingProvider, int userId);
        Task<List<DTOParkingProvider>> GetParkingLocationsByUser(int userId);
        Task<List<DTOParkingProvider>> GetAllParkingLocations();
        Task<string> DeleteParkingLocation(int uniqueId, int userId);
    }

}

