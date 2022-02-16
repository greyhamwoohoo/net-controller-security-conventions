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

        protected IEnumerable<Type> AcknowledgedAllowAnonymousControllers
        {
            get
            {
                var result = GetType().GetCustomAttributes<AcknowledgeAnonymousControllerAttribute>().Select(a => a.Controller);
                return result;
            }
        }

        protected IEnumerable<string> AcknowledgedAuthorizedMethods
        {
            get
            {
                var result = GetType()
                    .GetCustomAttributes<AcknowledgeAuthorizedActionMethodAttribute>()
                    .Select(a => $"{a.Controller.FullName}.{a.MethodName}");

                return result;
            }
        }

        protected IEnumerable<string> AcknowledgedAllowAnonymousMethods
        {
            get
            {
                var result = GetType()
                    .GetCustomAttributes<AcknowledgeAnonymousActionMethodAttribute>()
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
            var authorizedControllers = fromControllers.Where(ControllerIsAuthorized);
            return authorizedControllers;
        }

        protected IEnumerable<Type> GetAllowAnonymousControllers(IEnumerable<Type> fromControllers)
        {
            var allowAnonymousControllers = fromControllers.Where(ControllerIsAnonymous);
            return allowAnonymousControllers;
        }

        protected string ToAuthorizedAttribute(string methodFullName)
        {
            var parts = methodFullName.Split(".");
            var controllerName = string.Join(".", parts, parts.Length - 2, 1);
            var methodName = parts.Last();

            return $"[{typeof(AcknowledgeAuthorizedActionMethodAttribute).Name}(controller: typeof({controllerName}), methodName: \"{methodName}\", because: \"...reason...\")]";
        }

        protected string ToAnonymousAttribute(string methodFullName)
        {
            var parts = methodFullName.Split(".");
            var controllerName = string.Join(".", parts, parts.Length - 2, 1);
            var methodName = parts.Last();

            return $"[{typeof(AcknowledgeAnonymousActionMethodAttribute).Name}(controller: typeof({controllerName}), methodName: \"{methodName}\", because: \"...reason...\")]";
        }

        /* Determining whether a controller is Anonymous or Authorized statically is complicated by inheritance. Take this:
         * 
         * [Authorize]
         * public abstract MyBaseController : ControllerBase ...
         * 
         *    public string Get() { return "Hello"; }
         * 
         * [AllowAnonymous]
         * public MyController : MyBaseController ...
         *
         * typeof(MyController).GetCustomAttributes() will return a list of the attributes from MyController; then MyBaseController; and so on: the lower the index in the list, the higher the priority.
         *    Implication: it is not sufficient to look for an attribute of Authorize or AllowAnonymous; we need to look at which attribute comes first in the list to see which has priority. 
         * 
         * MyBaseController has a single action method with no explicit permission: it is an Authorized method in MyBaseController, but is Anonymous in MyController.
         */ 
        protected bool ControllerIsAuthorized(Type controllerType)
        {
            var attributes = controllerType.GetCustomAttributes();
            
            var firstAuthorizedAttribute = attributes.FirstOrDefault(a => a is AuthorizeAttribute);
            var authorizedIndex = attributes.ToList().IndexOf(firstAuthorizedAttribute);
            
            if(authorizedIndex == -1)
            {
                return false;
            }

            var firstAnonymousAttribute = attributes.FirstOrDefault(a => a is AllowAnonymousAttribute);
            var anonymousIndex = attributes.ToList().IndexOf(firstAnonymousAttribute);
            
            if(anonymousIndex == -1)
            {
                return authorizedIndex >= 0;
            }

            return authorizedIndex < anonymousIndex;
        }

        protected bool ControllerIsAnonymous(Type controllerType)
        {
            var attributes = controllerType.GetCustomAttributes();

            var firstAnonymousAttribute = attributes.FirstOrDefault(a => a is AllowAnonymousAttribute);
            var anonymousIndex = attributes.ToList().IndexOf(firstAnonymousAttribute);

            if(anonymousIndex == -1)
            {
                return false;
            }

            var firstAuthorizedAttribute = attributes.FirstOrDefault(a => a is AuthorizeAttribute);
            var authorizedIndex = attributes.ToList().IndexOf(firstAuthorizedAttribute);

            if (authorizedIndex == -1)
            {
                return anonymousIndex >= 0;
            }

            return anonymousIndex < authorizedIndex;
        }

        protected bool ActionMethodIsAnonymous(MethodInfo methodInfo)
        {
            var hasAnonymousAttribute = methodInfo.GetCustomAttributes<AllowAnonymousAttribute>().Count() > 0;
            if (hasAnonymousAttribute) return true;

            var hasAuthorizeAttribute = methodInfo.GetCustomAttributes<AuthorizeAttribute>().Count() > 0;
            if (hasAnonymousAttribute) return false;

            var controllerAsAnonymousAttribute = methodInfo.DeclaringType.GetCustomAttributes<AllowAnonymousAttribute>().Count() > 0;
            return controllerAsAnonymousAttribute;
        }

        protected bool ActionMethodIsAuthorized(MethodInfo methodInfo)
        {
            var hasAuthorizedAttribute = methodInfo.GetCustomAttributes<AuthorizeAttribute>().Count() > 0;
            if (hasAuthorizedAttribute) return true;

            var hasAnonymousAttribute = methodInfo.GetCustomAttributes<AllowAnonymousAttribute>().Count() > 0;
            if (hasAnonymousAttribute) return false;

            var controllerHasAuthorizeAttribute = methodInfo.DeclaringType.GetCustomAttributes<AuthorizeAttribute>().Count() > 0;
            return controllerHasAuthorizeAttribute;
        }

        protected MethodInfo[] GetControllerMethods(Type controllerType)
        {
            return controllerType
                .GetMethods(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        }
    }
}
