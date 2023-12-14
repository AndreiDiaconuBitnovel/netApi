using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.DataDB;

namespace WebApplication2.Services.TestDbServiceFolder
{
    public class TestDBService : ITestDBService
    {

        private readonly TeamDropDatabaseContext _testContext;

        public TestDBService(TeamDropDatabaseContext testContext)
        {
            _testContext = testContext;
        }
        public async Task<List<TestDb>> GetAllDataFromTestDbTable()
        {
            return await _testContext.TestDbs.ToListAsync();
        }
    }
}
