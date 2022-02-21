using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SecurityConventionsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ItsHierarchicalBaseController : ControllerBase
    {
        private readonly ILogger<ItsAuthorizedController> _logger;

        public ItsHierarchicalBaseController(ILogger<ItsAuthorizedController> logger)
        {
            _logger = logger;
        }

        public string Get() => "woo";
    }
}
