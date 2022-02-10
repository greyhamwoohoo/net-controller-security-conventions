using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SecurityConventions.UnitTests.Infrastructure
{
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
