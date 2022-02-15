using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityConventions.UnitTests.Infrastructure;
using SecurityConventionsApi.Controllers;
using System.Linq;

namespace SecurityConventions.UnitTests.CodingStandards
{
    [TestClass]
    [AcknowledgeAnonymousHttpMethod(controller: typeof(ItsAuthorizedController), methodName: "GetAnonymous", because: "this is an anonymous method in an authorized controller")]
    [AcknowledgeAuthorizedHttpMethod(controller: typeof(ItsAnonymousController), methodName: "GetAuthorized", because: "this is an authorized method in an anonymous controller")]
    public class HttpMethodConventionTests : SecurityConventionsTestBase
    {
        /// <summary>
        /// Every insecure method in an authorized controller must be explicitly acknowledged. 
        /// </summary>
        [TestMethod]
        public void AnonymousMethodsInAuthorizedControllerAreAcknowledged()
        {
            var expectedAcknowledgedAnonymousMethods = AuthorizedControllers
                .SelectMany(controller => controller.GetMethods()
                    .Where(HttpMethodIsAnonymous)
                    .Select(methodInfo => $"{controller.FullName}.{methodInfo.Name}"));

            var anonymousMethodsNotAcknowledged = expectedAcknowledgedAnonymousMethods
                .Except(AcknowledgedAllowAnonymousMethods);

            anonymousMethodsNotAcknowledged
                .Count()
                .Should()
                .Be(0, because: $"every anonymous HttpMethod in an authorized controller must be explicitly acknowledged. \r\n\r\nYou must add an [AcknowledgeAnonymousHttpMethod] attribute to this class for each of the following anomymous methods:\r\n\r\n{string.Join("\r\n", anonymousMethodsNotAcknowledged.Select(ToAnonymousAttribute))}\r\n\r\n");
        }

        /// <summary>
        /// Every acknowledged insecure method on an authorized controller must exist. This test enforces hygiene. 
        /// </summary>
        [TestMethod]
        public void AcknowledgedAnonymousHttpMethodExists()
        {
            var expectedAcknowledgedAnonymousMethods = AuthorizedControllers
                .SelectMany(controller => controller.GetMethods()
                    .Where(HttpMethodIsAnonymous)
                    .Select(methodInfo => $"{controller.FullName}.{methodInfo.Name}"));

            var anonymousMethodsThatDoNotExist= AcknowledgedAllowAnonymousMethods
                .Except(expectedAcknowledgedAnonymousMethods);

            anonymousMethodsThatDoNotExist
                .Count()
                .Should()
                .Be(0, because: $"\r\n\r\nThe following anonymous HttpMethods no longer exist. Remove the [AcknowledgeAnonymousHttpMethod] attribute from this class for each of the following methods:\r\n\r\n{string.Join("\r\n", anonymousMethodsThatDoNotExist.Select(ToAnonymousAttribute))}\r\n\r\n");
        }

        /// <summary>
        /// Every secure method in an anonymous controller must be explicitly acknowledged. 
        /// </summary>
        [TestMethod]
        public void AuthorizedMethodsInAnonymousControllerMustBeAcknowledged()
        {
            var expectedAcknowledgedAuthorizedMethods = AllowAnonymousControllers
                .SelectMany(controller => controller.GetMethods()
                    .Where(HttpMethodIsAuthorized)
                    .Select(methodInfo => $"{controller.FullName}.{methodInfo.Name}"));

            var authorizedMethodsNotAcknowledged = expectedAcknowledgedAuthorizedMethods
                .Except(AcknowledgedAuthorizedMethods);

            authorizedMethodsNotAcknowledged
                .Count()
                .Should()
                .Be(0, because: $"every authorized HttpMethod in an anonymous controller must be explicitly acknowledged. \r\n\r\nYou must add an [AcknowledgeAuthorizedHttpMethod] attribute to this class for each of the following methods:\r\n\r\n{string.Join("\r\n", authorizedMethodsNotAcknowledged.Select(ToAuthorizedAttribute))}\r\n\r\n");
        }
        
        /// <summary>
        /// Every acknowledged secure method in an anonymous controller must exist. This test enforced hygiene. 
        /// </summary>
        [TestMethod]
        public void AcknowledgedAuthorizedHttpMethodExists()
        {
            var expectedAcknowledgedAuthorizedMethods = AllowAnonymousControllers
                .SelectMany(controller => controller.GetMethods()
                    .Where(HttpMethodIsAuthorized)
                    .Select(methodInfo => $"{controller.FullName}.{methodInfo.Name}"));

            var authorizedMethodsThatDoNotExist = AcknowledgedAuthorizedMethods
                .Except(expectedAcknowledgedAuthorizedMethods);

            authorizedMethodsThatDoNotExist
                .Count()
                .Should()
                .Be(0, because: $"the following authorized HttpMethods no longer exist. \r\n\r\nYou must remove the [AcknowledgeAuthorizedHttpMethod] attribute from this class for each of the following methods:\r\n\r\n{string.Join("\r\n", authorizedMethodsThatDoNotExist.Select(ToAuthorizedAttribute))}\r\n\r\n");
        }
    }
}
