using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityConventions.UnitTests.Infrastructure;
using SecurityConventionsApi.Controllers;
using System;
using System.Linq;
using System.Reflection;

namespace SecurityConventions.UnitTests.CodingStandards
{
    [TestClass]
    public class ControllerConventionTests : SecurityConventionsTestBase
    {
        [TestMethod]
        [ControllersDataSource(fromAssemblyContaining: typeof(ItsAnonymousController))]
        public void ControllersHaveExplicitPermissions(Type controllerType)
        {
            // NOTE: The [Authorize] attribute can be specified many times; but [AllowAnonymous] only a maximum of once. 
            var allowAnonymousAttributeCount = controllerType.GetCustomAttributes<AllowAnonymousAttribute>(inherit: false).Count();
            var authorizeAttributeCount = controllerType.GetCustomAttributes<AuthorizeAttribute>(inherit: false).Count();

            var explicitPermissionsApplied = (allowAnonymousAttributeCount > 0) || (authorizeAttributeCount > 0);

            var bothPermissionsApplied = (allowAnonymousAttributeCount > 0) && (authorizeAttributeCount > 0);

            explicitPermissionsApplied.Should().BeTrue(because: "the Controller did not have either the [AllowAnonymous] or [Authorize] attribute applied. Permissions must be explicitly applied. ");

            bothPermissionsApplied.Should().BeFalse(because: "the Controller had both [AllowAnonymous] and [Authorize] attributes applied. Only one of those is allowed. ");
        }
    }
}
