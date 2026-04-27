using Models;

namespace SmartParking.Interfaces
{
    public interface IParkingProvider
    {
        Task<string> SaveParkingLocation(DTOParkingProvider DTOParkingProvider);
        Task<List<DTOParkingProvider>> GetParkingLocationsByUser(int userId);
        Task<List<DTOParkingProvider>> GetAllParkingLocations();
        Task<string> DeleteParkingLocation(int uniqueId);
    }

}

