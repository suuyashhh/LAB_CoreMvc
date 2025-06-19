using Models;
using System.Numerics;

namespace Lab_Mvc.Interfaces
{
    public interface ITest
    {

        Task<IEnumerable<DTOTest>> GetTests();
        Task<DTOTest> GetTestById(Int64 test_code);

        Task SaveTest(DTOTest test);
        Task EditTest(DTOTest test,long test_code);
        Task DeleteTest(long test_code);

    }
}
