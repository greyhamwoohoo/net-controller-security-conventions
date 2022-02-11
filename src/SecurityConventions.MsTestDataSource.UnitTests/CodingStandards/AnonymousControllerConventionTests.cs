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
    [AcknowledgeAnonymousController(Controller = typeof(ItsAnonymousController))]
    public class AnonymousControllerConventionTests : SecurityConventionsTestBase
    {
        /// <summary>
        /// Every Controller that has [AllowAnonymous] must be explicitly acknowledged. 
        /// </summary>
        /// <param name="controllerType"></param>
        [TestMethod]
        [AnonymousControllersDataSource(fromAssemblyContaining: typeof(ItsAnonymousController))]
        public void AnonymousControllersAreAcknowledgedAsAnonymous(Type controllerType)
        {
            var controllerIsAcknowledgedToBeAnonymous = GetType()
                .GetCustomAttributes<AcknowledgeAnonymousControllerAttribute>()
                .Select(a => a.Controller)
                .Contains(controllerType);

            controllerIsAcknowledgedToBeAnonymous.Should().BeTrue(because: "every controller that is [AllowAnonymous] must have a corresponding [AcknowledgeAnonymousController] attribute in the test project. This is to stop controllers accidentally been made anonymous when developing locally. ");
        }

        /// <summary>
        /// Every controller acknowledged to be anonymous must really exist. This test enforces hygiene. 
        /// </summary>
        /// <param name="attribute"></param>
        [TestMethod]
        [AcknowledgeAnonymousControllerAttributeDataSource(forAllAttributesOnClass: typeof(AnonymousControllerConventionTests))]
        public void AcknowlegedAnonymousControllerExists(AcknowledgeAnonymousControllerAttribute attribute)
        {
            var hasAllowAnonymousAttribute = attribute.Controller.GetCustomAttributes<AllowAnonymousAttribute>().Count() > 0;

            hasAllowAnonymousAttribute.Should().BeTrue(because: $"every Controller that is acknowledged to have the [AllowAnonymous] attribute must really have the [AllowAnonymous] attribute. If the controller is no longer anonymous, then remove the [AcknowledgeAnonymousController(Controller = typeof({attribute.Controller.Name}))] attribute from the top of this class. ");
        }
    }
}
