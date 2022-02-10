using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SecurityConventions.UnitTests.Infrastructure
{
    public class HttpMethodDataSource : Attribute, ITestDataSource
    {
        private List<Assembly> Assemblies;

        public HttpMethodDataSource(Type fromAssemblyContaining)
        {
            if (null == fromAssemblyContaining) throw new System.ArgumentNullException(nameof(fromAssemblyContaining));

            Assemblies = new List<Assembly>() { fromAssemblyContaining.Assembly };
        }

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            foreach (var assembly in Assemblies)
            {
                var controllers = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ControllerBase))).Where(FilterControllerByAttribute);

                foreach (var controller in controllers)
                {
                    var methods = controller
                        .GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                        .Where(m => m.GetCustomAttributes<HttpMethodAttribute>().Count() > 0);

                    foreach(var method in methods)
                    {
                        var result = new object[2] { controller, method };

                        yield return result;
                    }
                }
            }

            yield break;
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var className = ((Type)data[0]).FullName;
            return $"{className}.{((MethodInfo)data[1]).Name}";
        }

        /// <summary>
        /// Filter controllers based on attributes required for the database. By default: controllers are NOT filtered. Base classes can check for custom attributes
        /// and then indicate whether the Controller should be returned. 
        /// </summary>
        /// <param name="controllerType"></param>
        /// <returns></returns>
        public virtual bool FilterControllerByAttribute(Type controllerType)
        {
            return true;
        }
    }
}
