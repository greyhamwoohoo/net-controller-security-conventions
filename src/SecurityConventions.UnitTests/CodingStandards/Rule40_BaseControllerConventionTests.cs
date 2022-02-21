using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SecurityConventions.UnitTests.CodingStandards
{
    [TestClass]
    public class Rule40_BaseControllerConventionTests : SecurityConventionsTestBase
    {
        /// <summary>
        /// Every controller that acts as a base class (except ControllerBase) must be abstract.
        /// 
        /// RATIONALE: Base controllers are rarely instantiated on their own. 
        /// </summary>
        [TestMethod]
        public void BaseControllersMustBeMarkedAsAbstract()
        {
            var nonAbstractBaseControllers = Controllers
                .Where(c => c.BaseType != typeof(ControllerBase))
                .Select(c => c.BaseType)
                .Where(t => !t.IsAbstract);

            nonAbstractBaseControllers.Count().Should().Be(0, because: $"every Controller that is derived from must be marked as an abstract class. \r\nYou must add 'public abstract class...' to the following controllers: \r\n\r\n{string.Join("\r\n", nonAbstractBaseControllers.Select(c => c.FullName))}\r\n\r\n");
        }
    }
}
