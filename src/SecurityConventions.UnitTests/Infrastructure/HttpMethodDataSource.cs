using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SecurityConventions.UnitTests.Infrastructure
{
    /// <summary>
    /// DataSource to yield every method attributed with [HttpXXX] in every Controller in a given assembly. 
    /// </summary>
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
                var controllers = assembly
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(ControllerBase)))
                    .Where(ControllerIsRequiredByDataSource);

                foreach (var controller in controllers)
                {
                    var methods = controller
                        .GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                        .Where(MethodIsRequiredByDataSource)
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
            return $"{((Type)data[0]).FullName}.{((MethodInfo)data[1]).Name}";
        }

        /// <summary>
        /// Override in a derived class to select controllers required for this data source. 
        /// </summary>
        /// <param name="controllerType">The Controller type. </param>
        /// <returns>true if the Controller is to be returned by the DataSource; false otherwise. </returns>
        protected internal virtual bool ControllerIsRequiredByDataSource(Type controllerType)
        {
            return true;
        }

        /// <summary>
        /// Override in a derived class to select methods required for this data source. 
        /// </summary>
        /// <param name="methodInfo">The method attributes with [HttpXXX]. </param>
        /// <returns>true if the method is to be returned by the DataSource; false otherwise. </returns>
        protected internal virtual bool MethodIsRequiredByDataSource(MethodInfo methodInfo)
        {
            return true;
        }
    }
}
