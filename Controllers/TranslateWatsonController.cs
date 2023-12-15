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
            bool isValidInput = await _translateWatsonService.Validate(req);
            if (!isValidInput)
            {
                var err = new HttpRequestException(
                    "Language codes are not valid.",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
                throw err;
            }
            var result = await _translateWatsonService.GetTranslation(req);
            return result;
        }
    }
}
