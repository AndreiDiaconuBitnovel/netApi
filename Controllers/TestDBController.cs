using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models.TestDBModels;
using WebApplication2.Services.TestDbServiceFolder;
using System.Linq;


namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestDBController : ControllerBase
    {
        private readonly ITestDBService _testDBService;
        public TestDBController(ITestDBService testDBService) {
            _testDBService = testDBService;
        }

        [HttpGet, DisableRequestSizeLimit]
        [Route("getAllTestDbRecord")]
        public async Task<List<TestDBDto>> GetAllTestDbRecord()
        {
            var result = await _testDBService.GetAllDataFromTestDbTable();
            return result.Select(x => new TestDBDto
            {
                Id = x.Id,
                Name = x.Name,
                Prenume = x.Prenume
            }).ToList();

        }
    }
}
