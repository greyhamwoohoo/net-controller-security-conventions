using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SecurityConventionsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ItsHierarchicalDerivedController : ItsHierarchicalBaseController
    {
        private readonly ILogger<ItsAuthorizedController> _logger;

        public ItsHierarchicalDerivedController(ILogger<ItsAuthorizedController> logger) : base(logger)
        {
            _logger = logger;
        }
    }
}
