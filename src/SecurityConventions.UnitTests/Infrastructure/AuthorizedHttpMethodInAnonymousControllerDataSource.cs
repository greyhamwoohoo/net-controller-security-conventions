using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Reflection;

namespace SecurityConventions.UnitTests.Infrastructure
{
    /// <summary>
    /// DataSource to yield every method that is attributed with [HttpXXX] and [Authorize] in a Controller attributed with [AllowAnonymous]. 
    /// </summary>
    public class AuthorizedHttpMethodInAnonymousControllerDataSource : HttpMethodDataSource
    {
        public AuthorizedHttpMethodInAnonymousControllerDataSource(Type fromAssemblyContaining) : base(fromAssemblyContaining)
        { 
        }

        protected internal override bool ControllerIsRequiredByDataSource(Type controllerType)
        {
            var isAnonymousController = controllerType.GetCustomAttributes<AllowAnonymousAttribute>().Count() > 0;
            return isAnonymousController;
        }

        protected internal override bool MethodIsRequiredByDataSource(MethodInfo methodInfo)
        {
            var isAnonymousMethod = methodInfo.GetCustomAttributes<AuthorizeAttribute>().Count() > 0;
            return isAnonymousMethod;
        }
    }
}
