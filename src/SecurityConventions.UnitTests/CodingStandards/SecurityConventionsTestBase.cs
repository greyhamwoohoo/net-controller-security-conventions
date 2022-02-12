using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public abstract class SecurityConventionsTestBase
    {
        protected IEnumerable<Type> Controllers;
        protected IEnumerable<Type> AuthorizedControllers;
        protected IEnumerable<Type> AllowAnonymousControllers;

        protected internal IEnumerable<Type> AcknowledgedAllowAnonymousControllers
        {
            get
            {
                var result = GetType().GetCustomAttributes<AcknowledgeAnonymousControllerAttribute>().Select(a => a.Controller);
                return result;
            }
        }

        [TestInitialize]
        public void SetupControllerSecurityTests()
        {
            var candidateAssemblies = new Assembly[]
            {
                typeof(ItsAnonymousController).Assembly
            };

            Controllers = GetAllControllers(fromAssemblies: candidateAssemblies);
            AuthorizedControllers = GetAuthorizedControllers(fromControllers: Controllers);
            AllowAnonymousControllers = GetAllowAnonymousControllers(fromControllers: Controllers);
        }

        protected IEnumerable<Type> GetAllControllers(IEnumerable<Assembly> fromAssemblies)
        {
            var result = new List<Type>();

            foreach (var assembly in fromAssemblies)
            {
                result.AddRange(GetAllControllers(fromAssembly: assembly));
            };

            return result;
        }

        protected IEnumerable<Type> GetAllControllers(Assembly fromAssembly)
        {
            var controllers = fromAssembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ControllerBase)));
            return controllers;
        }
        protected IEnumerable<Type> GetAuthorizedControllers(IEnumerable<Type> fromControllers)
        {
            var authorizedControllers = fromControllers.Where(c => c.GetCustomAttributes<AuthorizeAttribute>().Count() > 0);
            return authorizedControllers;
        }
        protected IEnumerable<Type> GetAllowAnonymousControllers(IEnumerable<Type> fromControllers)
        {
            var allowAnonymousControllers = fromControllers.Where(c => c.GetCustomAttributes<AllowAnonymousAttribute>().Count() > 0);
            return allowAnonymousControllers;
        }
    }
}
