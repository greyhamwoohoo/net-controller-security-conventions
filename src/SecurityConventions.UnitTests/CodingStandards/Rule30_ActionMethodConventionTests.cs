using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityConventions.UnitTests.Infrastructure;
using SecurityConventionsApi.Controllers;
using System.Linq;

namespace SecurityConventions.UnitTests.CodingStandards
{
    [TestClass]
    [AcknowledgeAnonymousActionMethod(controller: typeof(ItsAuthorizedController), methodName: "GetAnonymous", because: "this is an anonymous method in an authorized controller")]
    [AcknowledgeAnonymousActionMethod(controller: typeof(ItsAuthorizedController), methodName: "GetAnonymousWithNoHttpMethodAttribute", because: "this is an anonymous method in an authorized controller but with no HttpMethodAttribute")]
    [AcknowledgeAuthorizedActionMethod(controller: typeof(ItsAnonymousController), methodName: "GetAuthorized", because: "this is an authorized method in an anonymous controller")]
    [AcknowledgeAuthorizedActionMethod(controller: typeof(ItsAnonymousController), methodName: "GetAuthorizedWithoutHttpMethodAttribute", because: "this is an authorized method in an anonymous controller but with no HttpMethodAttribute")]
    public class Rule30_ActionMethodConventionTests : SecurityConventionsTestBase
    {
        /// <summary>
        /// Every insecure method in an authorized controller must be explicitly acknowledged. 
        /// </summary>
        [TestMethod]
        public void AnonymousMethodsInAuthorizedControllerAreAcknowledged()
        {
            var expectedAcknowledgedAnonymousMethods = AuthorizedControllers
                .SelectMany(controller => controller.GetMethods()
                    .Where(ActionMethodIsAnonymous)
                    .Select(methodInfo => $"{controller.FullName}.{methodInfo.Name}"));

            var anonymousMethodsNotAcknowledged = expectedAcknowledgedAnonymousMethods
                .Except(AcknowledgedAllowAnonymousMethods);

            anonymousMethodsNotAcknowledged
                .Count()
                .Should()
                .Be(0, because: $"every anonymous action method in an authorized controller must be explicitly acknowledged. \r\n\r\nYou must add an [{typeof(AcknowledgeAnonymousActionMethodAttribute).Name}] attribute to this class for each of the following anomymous methods:\r\n\r\n{string.Join("\r\n", anonymousMethodsNotAcknowledged.Select(ToAnonymousAttribute))}\r\n\r\n");
        }

        /// <summary>
        /// Every acknowledged insecure method on an authorized controller must exist. This test enforces hygiene. 
        /// </summary>
        [TestMethod]
        public void AcknowledgedAnonymousActionMethodExists()
        {
            var expectedAcknowledgedAnonymousMethods = AuthorizedControllers
                .SelectMany(controller => controller.GetMethods()
                    .Where(ActionMethodIsAnonymous)
                    .Select(methodInfo => $"{controller.FullName}.{methodInfo.Name}"));

            var anonymousMethodsThatDoNotExist= AcknowledgedAllowAnonymousMethods
                .Except(expectedAcknowledgedAnonymousMethods);

            anonymousMethodsThatDoNotExist
                .Count()
                .Should()
                .Be(0, because: $"\r\n\r\nThe following anonymous action no longer exist. Remove the [{typeof(AcknowledgeAnonymousActionMethodAttribute).Name}] attribute from this class for each of the following methods:\r\n\r\n{string.Join("\r\n", anonymousMethodsThatDoNotExist.Select(ToAnonymousAttribute))}\r\n\r\n");
        }

        /// <summary>
        /// Every secure method in an anonymous controller must be explicitly acknowledged. 
        /// </summary>
        [TestMethod]
        public void AuthorizedActionMethodsInAnonymousControllerMustBeAcknowledged()
        {
            var expectedAcknowledgedAuthorizedMethods = AllowAnonymousControllers
                .SelectMany(controller => controller.GetMethods()
                    .Where(ActionMethodIsAuthorized)
                    .Select(methodInfo => $"{controller.FullName}.{methodInfo.Name}"));

            var authorizedMethodsNotAcknowledged = expectedAcknowledgedAuthorizedMethods
                .Except(AcknowledgedAuthorizedMethods);

            authorizedMethodsNotAcknowledged
                .Count()
                .Should()
                .Be(0, because: $"every authorized action methods in an anonymous controller must be explicitly acknowledged. \r\n\r\nYou must add an [{typeof(AcknowledgeAuthorizedActionMethodAttribute).Name}] attribute to this class for each of the following methods:\r\n\r\n{string.Join("\r\n", authorizedMethodsNotAcknowledged.Select(ToAuthorizedAttribute))}\r\n\r\n");
        }
        
        /// <summary>
        /// Every acknowledged secure method in an anonymous controller must exist. This test enforced hygiene. 
        /// </summary>
        [TestMethod]
        public void AcknowledgedAuthorizedActionMethodExists()
        {
            var expectedAcknowledgedAuthorizedMethods = AllowAnonymousControllers
                .SelectMany(controller => controller.GetMethods()
                    .Where(ActionMethodIsAuthorized)
                    .Select(methodInfo => $"{controller.FullName}.{methodInfo.Name}"));

            var authorizedMethodsThatDoNotExist = AcknowledgedAuthorizedMethods
                .Except(expectedAcknowledgedAuthorizedMethods);

            authorizedMethodsThatDoNotExist
                .Count()
                .Should()
                .Be(0, because: $"the following authorized action methods no longer exist. \r\n\r\nYou must remove the [{typeof(AcknowledgeAuthorizedActionMethodAttribute).Name}] attribute from this class for each of the following methods:\r\n\r\n{string.Join("\r\n", authorizedMethodsThatDoNotExist.Select(ToAuthorizedAttribute))}\r\n\r\n");
        }
    }
}
