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
        public string Get()
        {
            return "This is authorized";
        }
    }
}
