using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.DataDB;

namespace WebApplication2.Services.TestDbServiceFolder
{
    public class TestDBService : ITestDBService
    {

        private readonly TestContext _testContext;

        public TestDBService(TestContext testContext)
        {
            _testContext = testContext;
        }
        public async Task<List<TestDb>> GetAllDataFromTestDbTable()
        {
            return await _testContext.TestDbs.ToListAsync();
        }
    }
}
