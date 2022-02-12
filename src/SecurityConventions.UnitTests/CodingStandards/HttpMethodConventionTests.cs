using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityConventions.UnitTests.Infrastructure;
using SecurityConventionsApi.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SecurityConventions.UnitTests.CodingStandards
{
    [TestClass]
    [AcknowledgeAnonymousHttpMethod(controller: typeof(ItsAuthorizedController), methodName: "GetAnonymous", because: "this is an anonymous method in an authorized controller")]
    [AcknowledgeAuthorizedHttpMethod(controller: typeof(ItsAnonymousController), methodName: "GetAuthorized", because: "this is an authorized method in an anonymous controller")]
    public class HttpMethodConventionTests : SecurityConventionsTestBase
    {
        /// <summary>
        /// Every insecure method in an [Authorize] controller must be explicitly acknowledged. 
        /// </summary>
        [TestMethod]
        public void AnonymousMethodsInAuthorizedControllerAreAcknowledged()
        {
            var expectedAcknowledgedAnonymousMethods = AuthorizedControllers
                .SelectMany(controller => controller.GetMethods()
                    .Where(MethodIsHttpMethod)
                    .Where(HttpMethodIsAnonymous)
                    .Select(methodInfo => $"{controller.FullName}.{methodInfo.Name}"));

            var anonymousMethodsNotAcknowledged = expectedAcknowledgedAnonymousMethods
                .Except(AcknowledgedAllowAnonymousMethods);

            anonymousMethodsNotAcknowledged
                .Count()
                .Should()
                .Be(0, because: $"every [AllowAnonymous] HttpMethod in an [Authorize]'d Controller must be explicitly acknowledged. \r\n\r\nYou must add an [AcknowledgeAnonymousHttpMethod] attribute to this class for each of the following anomymous methods:\r\n\r\n{string.Join("\r\n", anonymousMethodsNotAcknowledged.Select(ToAnonymousAttribute))}\r\n\r\n");
        }

        /// <summary>
        /// Every acknowledged insecure method on an [Authorize] controller must exist. This test enforces hygiene. 
        /// </summary>
        /// <param name="attribute"></param>
        [TestMethod]
        public void AcknowledgedAnonymousHttpMethodExists()
        {
            var expectedAcknowledgedAnonymousMethods = AuthorizedControllers
                .SelectMany(controller => controller.GetMethods()
                    .Where(MethodIsHttpMethod)
                    .Where(HttpMethodIsAnonymous)
                    .Select(methodInfo => $"{controller.FullName}.{methodInfo.Name}"));

            var anonymousMethodsThatDoNotExist= AcknowledgedAllowAnonymousMethods
                .Except(expectedAcknowledgedAnonymousMethods);

            anonymousMethodsThatDoNotExist
                .Count()
                .Should()
                .Be(0, because: $"\r\n\r\nThe following methods no longer exist. Remove the [AcknowledgeAnonymousHttpMethod] attribute from this class for each of the following acknowledgements:\r\n\r\n{string.Join("\r\n", anonymousMethodsThatDoNotExist.Select(ToAnonymousAttribute))}\r\n\r\n");
        }

            
        /// <summary>
        /// Every secure method in an anonymous controller must be explicitly acknowledged. 
        /// </summary>
        /// <param name="controllerType">Controller type</param>
        /// <param name="methodInfo">Method</param>
        [TestMethod]
        public void AuthorizedMethodsInAnonymousControllerMustBeAcknowledged()
        {
            var expectedAcknowledgedAuthorizedMethods = AllowAnonymousControllers
                .SelectMany(controller => controller.GetMethods()
                    .Where(MethodIsHttpMethod)
                    .Where(HttpMethodIsAuthorized)
                    .Select(methodInfo => $"{controller.FullName}.{methodInfo.Name}"));

            var authorizedMethodsNotAcknowledged = expectedAcknowledgedAuthorizedMethods
                .Except(AcknowledgedAuthorizedMethods);

            authorizedMethodsNotAcknowledged
                .Count()
                .Should()
                .Be(0, because: $"every [Authorize] HttpMethod in an [AllowAnonymous] Controller must be explicitly acknowledged. \r\n\r\nYou must add an [AcknowledgeAuthorizedHttpMethod] attribute to this class for each of the following anomymous methods:\r\n\r\n{string.Join("\r\n", authorizedMethodsNotAcknowledged.Select(ToAuthorizedAttribute))}\r\n\r\n");
        }
        
        /// <summary>
        /// Every acknowledged secure method in an anonymous controller must exist. This test enforced hygiene. 
        /// </summary>
        /// <param name="attribute"></param>
        [TestMethod]
        public void AcknowledgedAuthorizedHttpMethodExists()
        {
            var expectedAcknowledgedAuthorizedMethods = AllowAnonymousControllers
                .SelectMany(controller => controller.GetMethods()
                    .Where(MethodIsHttpMethod)
                    .Where(HttpMethodIsAuthorized)
                    .Select(methodInfo => $"{controller.FullName}.{methodInfo.Name}"));

            var authorizedMethodsThatDoNotExist = AcknowledgedAuthorizedMethods
                .Except(expectedAcknowledgedAuthorizedMethods);

            authorizedMethodsThatDoNotExist
                .Count()
                .Should()
                .Be(0, because: $"the following [Authorize]'d methods no longer exist. \r\n\r\nYou must remove the [AcknowledgeAuthorizedHttpMethod] attribute from this class for each of the following anomymous methods:\r\n\r\n{string.Join("\r\n", authorizedMethodsThatDoNotExist.Select(ToAuthorizedAttribute))}\r\n\r\n");
        }

        private bool HttpMethodIsAnonymous(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<AllowAnonymousAttribute>().Count() > 0;
        }

        private bool HttpMethodIsAuthorized(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<AuthorizeAttribute>().Count() > 0;
        }

        private bool MethodIsHttpMethod(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<HttpMethodAttribute>().Count() > 0;
        }

        private IEnumerable<string> AcknowledgedAllowAnonymousMethods
        {
            get
            {
                var result = GetType()
                    .GetCustomAttributes<AcknowledgeAnonymousHttpMethodAttribute>()
                    .Select(a => $"{a.Controller.FullName}.{a.MethodName}");

                return result;
            }
        }

        private IEnumerable<string> AcknowledgedAuthorizedMethods
        {
            get
            {
                var result = GetType()
                    .GetCustomAttributes<AcknowledgeAuthorizedHttpMethodAttribute>()
                    .Select(a => $"{a.Controller.FullName}.{a.MethodName}");

                return result;
            }
        }

        private string ToAnonymousAttribute(string methodFullName)
        {
            var parts = methodFullName.Split(".");
            var controllerName = string.Join(".", parts, parts.Length - 2, 1);
            var methodName = parts.Last();

            return $"[AcknowledgeAnonymousHttpMethod(controller: typeof({controllerName}), methodName: \"{methodName}\", because: \"...reason...\")]";
        }

        private string ToAuthorizedAttribute(string methodFullName)
        {
            var parts = methodFullName.Split(".");
            var controllerName = string.Join(".", parts, parts.Length - 2, 1);
            var methodName = parts.Last();

            return $"[AcknowledgeAuthorizedHttpMethod(controller: typeof({controllerName}), methodName: \"{methodName}\", because: \"...reason...\")]";
        }
    }
}
