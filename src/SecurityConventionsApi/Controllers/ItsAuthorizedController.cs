using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SecurityConventionsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ItsAuthorizedController : ControllerBase
    {

        private readonly ILogger<ItsAuthorizedController> _logger;

        public ItsAuthorizedController(ILogger<ItsAuthorizedController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public string Get()
        {
            return "This is authorized";
        }

        [HttpGet]
        [AllowAnonymous]
        public string GetAnonymous()
        {
            return "This is an anonymous method in an Authorized controller";
        }

        public string ThisIsNotAHttpMethod()
        {
            return "Indeed";
        }
    }
}
