using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityConventionsApi.Controllers;
using System.Linq;
using System.Reflection;

namespace SecurityConventions.UnitTests.CodingStandards
{
    [TestClass]
    public class Rule10_ControllerConventionTests : SecurityConventionsTestBase
    {
        /// <summary>
        /// Every controller must have an [AllowAnonymous] or [Authorize] attribute
        /// </summary>
        [TestMethod]
        public void ControllersMustHaveExplicitPermissions()
        {
            var controllersThatDoNotHaveExplicitPermissions = Controllers
                .Except(AllowAnonymousControllers)
                .Except(AuthorizedControllers);

            controllersThatDoNotHaveExplicitPermissions.Count().Should().Be(0, because: $"every Controller must be marked with one of: [AllowAnonymous] or [Authorize]. \r\nYou must add either [AllowAnonymous] or [Authorize] to the following controllers: \r\n\r\n{string.Join("\r\n", controllersThatDoNotHaveExplicitPermissions.Select(c => c.FullName))}\r\n\r\n");
        }
    }
}
