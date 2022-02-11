using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityConventions.UnitTests.Infrastructure;
using SecurityConventionsApi.Controllers;
using System;
using System.Linq;
using System.Reflection;

namespace SecurityConventions.UnitTests.CodingStandards
{
    [TestClass]
    [AcknowledgeAnonymousHttpMethod(Controller = typeof(ItsAuthorizedController), MethodName = "GetAnonymous")]
    [AcknowledgeAuthorizedHttpMethod(Controller = typeof(ItsAnonymousController), MethodName = "GetAuthorized")]
    public class HttpMethodConventionTests : SecurityConventionsTestBase
    {
        /*
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
        */

        /// <summary>
        /// Every insecure method in an [Authorize] controller must be explicitly acknowledged. 
        /// </summary>
        /// <param name="controllerType">Controller type</param>
        /// <param name="methodInfo">Method</param>
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

        /// <summary>
        /// Every acknowledged insecure method on an [Authorize] controller must exist. This test enforces hygiene. 
        /// </summary>
        /// <param name="attribute"></param>
        [TestMethod]
        [AcknowledgeAnonymousHttpMethodAttributeDataSource(forAllAttributesOnClass: typeof(HttpMethodConventionTests))]
        public void AcknowledgedAnonymousHttpMethodExists(AcknowledgeAnonymousHttpMethodAttribute attribute)
        {
            var method = attribute.Controller.GetMethod(attribute.MethodName);

            method.Should().NotBeNull(because: $"the method called {attribute.MethodName} is expected to exist on the class {attribute.Controller.FullName} but it does not. Either remove the [AcknowledgeAnonymousHttpMethod] from this class that refers to that controller or method; or correct it. ");
        }

        /// <summary>
        /// Every secure method in an anonymous controller must be explicitly acknowledged. 
        /// </summary>
        /// <param name="controllerType">Controller type</param>
        /// <param name="methodInfo">Method</param>
        [TestMethod]
        [AuthorizedHttpMethodInAnonymousControllerDataSource(fromAssemblyContaining: typeof(ItsAnonymousController))]
        public void AuthorizedMethodsInAnonymousControllerMustBeAcknowledged(Type controllerType, MethodInfo methodInfo)
        {
            var methodIsAcknowledgedToBeAuthorizedInAnonymousController = GetType()
                .GetCustomAttributes<AcknowledgeAuthorizedHttpMethodAttribute>()
                .Where(a => a.MethodName == methodInfo.Name)
                .Count() > 0;

            methodIsAcknowledgedToBeAuthorizedInAnonymousController.Should().BeTrue(because: $"every [Authorize] HttpMethod in an [AllowAnonymous] Controller must be explicitly acknowledged. This is to stop methods accidentally been made anonymous when developing locally. You must add the following to the top of this class: [AcknowledgeAuthorizedHttpMethod(Controller = typeof({controllerType.Name}), MethodName = \"{methodInfo.Name}\")]");
        }

        /// <summary>
        /// Every acknowledged secure method in an anonymous controller must exist. This test enforced hygiene. 
        /// </summary>
        /// <param name="attribute"></param>
        [TestMethod]
        [AcknowledgeAuthorizedHttpMethodAttributeDataSource(forAllAttributesOnClass: typeof(HttpMethodConventionTests))]
        public void AcknowledgedAuthorizedHttpMethodExists(AcknowledgeAuthorizedHttpMethodAttribute attribute)
        {
            var method = attribute.Controller.GetMethod(attribute.MethodName);

            method.Should().NotBeNull(because: $"the method called {attribute.MethodName} is expected to exist on the class {attribute.Controller.FullName} but it does not. Either remove the [AcknowledgeAuthorizeHttpMethod] from this class that refers to that controller or method; or correct it. ");
        }
    }
}
