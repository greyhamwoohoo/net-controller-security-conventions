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

        /// <summary>
        /// Uncomment this to test Rule60. No new methods should be defined in Abstract Controllers. 
        /// </summary>
        /// <returns></returns>
        // public string TestGet() => "woo";


        /// <summary>
        /// This tests Rule60. Public overridden methods *ARE* allowed in abstract controllers for specialization. 
        /// </summary>
        /// <returns></returns>
        public override AcceptedResult Accepted()
        {
            return base.Accepted();
        }
    }
}
