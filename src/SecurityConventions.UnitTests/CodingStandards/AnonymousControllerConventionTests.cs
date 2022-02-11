using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
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
    public class AnonymousControllerConventionTests : SecurityConventionsTestBase
    {
        [TestMethod]
        [AnonymousControllersDataSource(fromAssemblyContaining: typeof(ItsAnonymousController))]
        public void AnonymousControllersAreAcknowledgedAsAnonymous(Type controllerType)
        {
            var controlledIsAcknowledgedToBeAnonymous = AcknowledgedAnonymousControllers.Contains(controllerType);

            controlledIsAcknowledgedToBeAnonymous.Should().BeTrue(because: "every controller that is [AllowAnonymous] must have a corresponding [AcknowledgeAnonymousController] attribute in the test project. This is to stop controllers accidentally been made anonymous when developing locally. ");
        }

        [TestMethod]
        [AcknowledgeAnonymousControllerAttributeDataSource(forAllAttributesOnClass: typeof(AnonymousControllerConventionTests))]
        public void AcknowledgedAnonymousControllersReferToRealAnonymousControllers(AcknowledgeAnonymousControllerAttribute attribute)
        {
            var hasAllowAnonymousAttribute = attribute.Controller.GetCustomAttributes<AllowAnonymousAttribute>().Count() > 0;

            hasAllowAnonymousAttribute.Should().BeTrue(because: $"every Controller that is acknowledged to have the [AllowAnonymous] attribute must really have the [AllowAnonymous] attribute. If the controller is no longer anonymous, then remove the [AcknowledgeAnonymousController(Controller = typeof({attribute.Controller.Name}))] attribute from the top of this class. ");
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
