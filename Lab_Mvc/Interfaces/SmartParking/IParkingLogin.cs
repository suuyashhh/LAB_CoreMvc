using Models;

namespace SmartParking.Interfaces
{
    public interface IParkingLogin
    {
        Task<DTOParkingLogin> Login(string phone ,string pass);
    }
}
