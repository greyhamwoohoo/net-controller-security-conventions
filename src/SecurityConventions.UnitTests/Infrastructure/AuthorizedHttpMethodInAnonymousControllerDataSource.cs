using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Reflection;

namespace SecurityConventions.UnitTests.Infrastructure
{
    public class AuthorizedHttpMethodInAnonymousControllerDataSource : HttpMethodDataSource
    {
        public AuthorizedHttpMethodInAnonymousControllerDataSource(Type fromAssemblyContaining) : base(fromAssemblyContaining)
        { 
        }

        public override bool FilterControllerByAttribute(Type controllerType)
        {
            var isAnonymousController = controllerType.GetCustomAttributes<AllowAnonymousAttribute>().Count() > 0;
            return isAnonymousController;
        }

        public override bool FilterMethodByAttribute(MethodInfo methodInfo)
        {
            var isAnonymousMethod = methodInfo.GetCustomAttributes<AuthorizeAttribute>().Count() > 0;
            return isAnonymousMethod;
        }
    }
}
