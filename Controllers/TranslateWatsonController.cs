using Microsoft.AspNetCore.Mvc;
using WebApplication2.Services.TranslateWatson;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslateWatsonController : ControllerBase
    {
        private readonly ITranslateWatsonService _translateWatsonService;
        public TranslateWatsonController(ITranslateWatsonService translateWatsonService)
        {
            _translateWatsonService = translateWatsonService;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("translate")]
        public async Task<TranslateWatsonRes> GetAllTranslateLanguageRecord(TranslateWatsonReq req)
        {
            var result = await _translateWatsonService.GetTranslation(req);
            return result;
        }
    }
}
