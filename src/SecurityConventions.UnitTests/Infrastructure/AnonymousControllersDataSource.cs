using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Reflection;

namespace SecurityConventions.UnitTests.Infrastructure
{
    /// <summary>
    /// DataSource to yield every Controller attributed with [AllowAnonymous] in a given assembly. 
    /// </summary>
    public class AnonymousControllersDataSource : ControllersDataSource
    {
        public AnonymousControllersDataSource(Type fromAssemblyContaining) : base(fromAssemblyContaining: fromAssemblyContaining)
        {
        }

        public override bool FilterControllerByAttribute(Type controllerType)
        {
            var isAnonymousController = controllerType.GetCustomAttributes<AllowAnonymousAttribute>().Count() > 0;
            return isAnonymousController;
        }
    }
}
