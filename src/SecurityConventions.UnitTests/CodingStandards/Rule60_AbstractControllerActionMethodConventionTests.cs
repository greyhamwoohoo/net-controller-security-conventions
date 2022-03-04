using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SecurityConventions.UnitTests.CodingStandards
{
    [TestClass]
    public class Rule60_AbstractControllerActionMethodConventionTests : SecurityConventionsTestBase
    {
        /// <summary>
        /// Every Abstract Controller must have no public methods.
        /// 
        /// RATIONALE: By convention, abstract controllers must only contain private, protected or public override methods (to specialize base behavior). 
        ///
        /// In .Net land, the following considerations arise from public methods in abstract controllers:
        /// 
        /// 1. If the method does not specify [AllowAnomymous] or [Authorize], the method permissions are set by the derived controller permissions. 
        ///    NOTE: The derived controller is allowed to override the default permissions of the abstract controller, therefore changing the visiblity of the method. 
        ///
        /// Within the context of these rules, the consequences of allowing public methods on abstract classes become complicated:
        /// 
        /// 1. Which controller "OWNS" the method? 
        ///    1a. Do we acknowledge anonymous methods in only derived authorized controllers
        ///    1b. Do we acknowledge authorized methods in only derived anonymous controllers?
        ///    1c. Do we enforce specific permissions on every public method? 
        ///    1d. Do we ensure that the abstract controller has permissions that are never changed in derived controllers? 
        ///
        /// It gets complicated. So we make the problem go away with this Rule. If this Rule is not appropriate for your use case, remove it and perhaps address the considerations above. 
        /// </summary>
        [TestMethod]
        public void AbstractControllersMustHaveNoNewPublicMethods()
        {
            var publicMethods = AbstractControllers
                .SelectMany(controller => GetControllerMethods(controller)
                    .Where(MethodIsPublic)
                    .Where(MethodIsNotOverridden)
                    .Select(methodInfo => $"{controller.FullName}.{methodInfo.Name}"));

            publicMethods
                .Count()
                .Should()
                .Be(0, because: $"there should be no public methods in Abstract Controllers. \r\n\r\nYou must either remove or make the following methods protected, internal or private:\r\n\r\n{string.Join("\r\n", publicMethods)}\r\n\r\n");

        }
    }
}
