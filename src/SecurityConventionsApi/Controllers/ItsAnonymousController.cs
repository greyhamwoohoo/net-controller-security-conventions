using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SecurityConventionsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class ItsAnonymousController : ControllerBase
    {

        private readonly ILogger<ItsAnonymousController> _logger;

        public ItsAnonymousController(ILogger<ItsAnonymousController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public string Get()
        {
            return "This is anonymous";
        }

        [HttpGet]
        [Authorize]
        public string GetAuthorized()
        {
            return "This is authorized";
        }

        public string ThisIsNotAHttpMethod()
        {
            return "Indeed";
        }
    }
}
