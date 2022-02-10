using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityConventions.UnitTests.Infrastructure;
using SecurityConventionsApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SecurityConventions.UnitTests.CodingStandards
{
    [TestClass]
    [AcknowledgeAnonymousController(Controller = typeof(ItsAnonymousController))]
    public class AnonymousControllerTests : SecurityConventionsTestBase
    {
        [TestMethod]
        [AnonymousControllersDataSource(fromAssemblyContaining: typeof(ItsAnonymousController))]
        public void AnonymousControllersAreAcknowledgedAsAnonymous(Type controllerType)
        {
            var controlledIsAcknowledgedToBeAnonymous = AcknowledgedAnonymousControllers.Contains(controllerType);

            controlledIsAcknowledgedToBeAnonymous.Should().BeTrue(because: "every controller that is [AllowAnonymous] must have a corresponding [AcknowledgeAnonymousController] attribute in the test project. This is to stop controllers accidentally been made anonymous when developing locally. ");
        }

        private IEnumerable<Type> AcknowledgedAnonymousControllers
        {
            get
            {
                var result = GetType().GetCustomAttributes<AcknowledgeAnonymousControllerAttribute>().Select(a => a.Controller);
                return result;
            }
        }
    }
}
