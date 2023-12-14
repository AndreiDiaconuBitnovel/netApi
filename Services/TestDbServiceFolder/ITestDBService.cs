using WebApplication2.DataDB;

namespace WebApplication2.Services.TestDbServiceFolder
{
    public interface ITestDBService
    {
        Task<List<TestDb>> GetAllDataFromTestDbTable();
    }
}
