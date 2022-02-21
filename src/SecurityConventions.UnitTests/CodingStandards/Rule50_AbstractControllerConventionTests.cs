using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;

namespace SecurityConventions.UnitTests.CodingStandards
{
    [TestClass]
    public class Rule50_AbstractControllerConventionTests: SecurityConventionsTestBase
    {
        /// <summary>
        /// Every abstract controller must NOT specify [AllowAnonymous] or [Authorize]
        /// 
        /// RATIONALE: Derived controllers explicitly specify [AllowAnonymous] or [Authorize] so the attribute is redundant on abstract controllers. 
        /// </summary>
        [TestMethod]
        public void AbstractControllersMustNotSpecifySecurityAttributes()
        {
            var abstractControllersThatSpecifySecurityAttributes = AbstractControllers
                .Where(c => c.GetCustomAttributes<AllowAnonymousAttribute>().Count() > 0 || c.GetCustomAttributes<AuthorizeAttribute>().Count() > 0);

            abstractControllersThatSpecifySecurityAttributes.Count().Should().Be(0, because: $"every abstract Controller must not specify [AllowAnonymous] or [Authorize] because the derived Controllers specify their own security. \r\nYou must remove security attributes from the following controllers: \r\n\r\n{string.Join("\r\n", abstractControllersThatSpecifySecurityAttributes.Select(c => c.FullName))}\r\n\r\n");
        }
    }
}
