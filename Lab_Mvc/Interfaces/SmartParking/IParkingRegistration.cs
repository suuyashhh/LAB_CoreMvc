using Models.SmartParking;

namespace SmartParking.Interfaces
{
    public interface IParkingRegistration
    {
        Task<int> Register(DTOParkingRegistration registration);
    }
}
