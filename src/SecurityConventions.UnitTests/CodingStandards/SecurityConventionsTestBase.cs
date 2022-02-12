using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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

        protected internal IEnumerable<Type> AcknowledgedAllowAnonymousControllers
        {
            get
            {
                var result = GetType().GetCustomAttributes<AcknowledgeAnonymousControllerAttribute>().Select(a => a.Controller);
                return result;
            }
        }

        protected internal IEnumerable<string> AcknowledgedAuthorizedMethods
        {
            get
            {
                var result = GetType()
                    .GetCustomAttributes<AcknowledgeAuthorizedHttpMethodAttribute>()
                    .Select(a => $"{a.Controller.FullName}.{a.MethodName}");

                return result;
            }
        }

        protected internal IEnumerable<string> AcknowledgedAllowAnonymousMethods
        {
            get
            {
                var result = GetType()
                    .GetCustomAttributes<AcknowledgeAnonymousHttpMethodAttribute>()
                    .Select(a => $"{a.Controller.FullName}.{a.MethodName}");

                return result;
            }
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

        protected internal string ToAuthorizedAttribute(string methodFullName)
        {
            var parts = methodFullName.Split(".");
            var controllerName = string.Join(".", parts, parts.Length - 2, 1);
            var methodName = parts.Last();

            return $"[AcknowledgeAuthorizedHttpMethod(controller: typeof({controllerName}), methodName: \"{methodName}\", because: \"...reason...\")]";
        }

        protected internal string ToAnonymousAttribute(string methodFullName)
        {
            var parts = methodFullName.Split(".");
            var controllerName = string.Join(".", parts, parts.Length - 2, 1);
            var methodName = parts.Last();

            return $"[AcknowledgeAnonymousHttpMethod(controller: typeof({controllerName}), methodName: \"{methodName}\", because: \"...reason...\")]";
        }

        protected internal bool HttpMethodIsAnonymous(MethodInfo methodInfo) => methodInfo.GetCustomAttributes<AllowAnonymousAttribute>().Count() > 0;

        protected internal bool HttpMethodIsAuthorized(MethodInfo methodInfo) => methodInfo.GetCustomAttributes<AuthorizeAttribute>().Count() > 0;

        protected internal bool MethodIsHttpMethod(MethodInfo methodInfo) => methodInfo.GetCustomAttributes<HttpMethodAttribute>().Count() > 0;
    }
}
