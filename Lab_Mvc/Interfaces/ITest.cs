using Models;
using System.Numerics;

namespace Lab_Mvc.Interfaces
{
    public interface ITest
    {

        Task<IEnumerable<DTOTest>> GetTests(int comId);
        Task<DTOTest> GetTestById(Int64 test_code, int comId);

        Task SaveTest(DTOTest test);
        Task EditTest(DTOTest test,long test_code);
        Task DeleteTest(long test_code, int comId);

    }
}
