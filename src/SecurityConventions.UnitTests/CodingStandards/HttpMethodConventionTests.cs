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
    [AcknowledgeAnonymousHttpMethod(Controller = typeof(ItsAnonymousController), MethodName = "GetAnonymous")]
    public class HttpMethodConventionTests : SecurityConventionsTestBase
    {
        [TestMethod]
        [HttpMethodDataSource(fromAssemblyContaining: typeof(ItsAnonymousController))]
        public void HttpMethodsHaveExplicitPermissions(Type controllerType, MethodInfo methodInfo)
        {
            // NOTE: The [Authorize] attribute can be specified many times; but [AllowAnonymous] only a maximum of once. 
            var allowAnonymousAttributeCount = methodInfo.GetCustomAttributes<AllowAnonymousAttribute>(inherit: false).Count();
            var authorizeAttributeCount = methodInfo.GetCustomAttributes<AuthorizeAttribute>(inherit: false).Count();

            var explicitPermissionsApplied = (allowAnonymousAttributeCount > 0) || (authorizeAttributeCount > 0);

            var bothPermissionsApplied = (allowAnonymousAttributeCount > 0) && (authorizeAttributeCount > 0);

            explicitPermissionsApplied.Should().BeTrue(because: "the HttpMethod did not have either the [AllowAnonymous] or [Authorize] attribute applied. Permissions must be explicitly applied. ");

            bothPermissionsApplied.Should().BeFalse(because: "the HttpMethod had both [AllowAnonymous] and [Authorize] attributes applied. Only one of those is allowed. ");
        }

        [TestMethod]
        [AnonymousHttpMethodInAuthorizedControllerDataSource(fromAssemblyContaining: typeof(ItsAnonymousController))]
        public void AnonymousMethodsInAuthorizedControllerAreAcknowleged(Type controllerType, MethodInfo methodInfo)
        {
            var methodIsAcknowledgedToBeAnonymousInAuthorizedController = GetType()
                .GetCustomAttributes<AcknowledgeAnonymousHttpMethodAttribute>()
                .Where(a => a.MethodName == methodInfo.Name)
                .Count() > 0;

            methodIsAcknowledgedToBeAnonymousInAuthorizedController.Should().BeTrue(because: $"every [AllowAnonymous] HttpMethod in an [Authorize]'d Controller must be explicitly acknowledged. This is to stop methods accidentally been made anonymous when developing locally. You must add the following to the top of this class: [AcknowledgeAnonymousHttpMethod(Controller = typeof({controllerType.Name}), MethodName = \"{methodInfo.Name}\")]");
        }
    }
}
