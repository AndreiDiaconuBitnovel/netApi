using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models.TestDBModels;
using WebApplication2.Services.TestDbServiceFolder;
using System.Linq;
using WebApplication2.DataDB;
using WebApplication2.Services.TranslateLanguageFolder;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslateLanguageController : ControllerBase
    {
        private readonly ITranslateLanguageService _translateLanguageService;
        public TranslateLanguageController(ITranslateLanguageService translateLanguageService)
        {
            _translateLanguageService = translateLanguageService;
        }

        [HttpGet, DisableRequestSizeLimit]
        [Route("getAllTranslateLanguageRecord")]
        public async Task<List<TranslateLanguage>> GetAllTranslateLanguageRecord()
        {
            var result = await _translateLanguageService.GetAllDataFromTranslateLanguageTable();
            return result.Select(x => new TranslateLanguage
            {
                Code = x.Code,
                NameInternational = x.NameInternational,
                NameLocal = x.NameLocal
            }).ToList();
        }
    }
}
