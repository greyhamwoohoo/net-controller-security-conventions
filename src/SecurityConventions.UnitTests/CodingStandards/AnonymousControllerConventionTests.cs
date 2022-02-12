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
    [AcknowledgeAnonymousController(controller: typeof(ItsAnonymousController), because: "this is an anonymous controller")]
    public class AnonymousControllerConventionTests : SecurityConventionsTestBase
    {
        /// <summary>
        /// Every Controller that has [AllowAnonymous] must be explicitly acknowledged. 
        /// </summary>
        [TestMethod]
        public void AnonymousControllersAreAcknowledgedAsAnonymous()
        {
            var controllersNotAcknowledgedAsAnonymous = AllowAnonymousControllers
                .Except(AcknowledgedAllowAnonymousControllers);

            controllersNotAcknowledgedAsAnonymous.Count().Should().Be(0, because: $"every controller that has the [AllowAnonymous] attribute must be acknowledged here. \r\n\r\nIf you intend these controllers to be anonymous, then add the following attributes to the top of this class:\r\n\r\n{string.Join("\r\n", controllersNotAcknowledgedAsAnonymous.Select(c => $"[AcknowledgeAnonymousController(controller: typeof({c.Name}), because: \"...reason...\")]"))}\r\n\r\n");
        }

        /// <summary>
        /// Every controller acknowledged to be anonymous must really exist. This test enforces hygiene. 
        /// </summary>
        /// <param name="attribute"></param>
        [TestMethod]
        public void AcknowlegedAnonymousControllerExists()
        {
            var acknowledgedAnonymousControllersThatDoNotExist = AcknowledgedAllowAnonymousControllers
                .Except(AllowAnonymousControllers);

            acknowledgedAnonymousControllersThatDoNotExist.Count().Should().Be(0, because: $"one or more acknowledged anonymous controllers either no longer exist or are no longer anonymous. \r\n\r\nIf you intend these controllers to no longer be anonymous, remove the following attributes from the top of this class:\r\n\r\n{string.Join("\r\n", acknowledgedAnonymousControllersThatDoNotExist.Select(c => $"[AcknowledgeAnonymousController(controller: typeof({c.Name}), because: \"...reason...\")]"))}\r\n\r\n");
        }

        private IEnumerable<Type> AcknowledgedAllowAnonymousControllers

        {
            get
            {
                var result = GetType().GetCustomAttributes<AcknowledgeAnonymousControllerAttribute>().Select(a => a.Controller);
                return result;
            }
        }
    }
}
