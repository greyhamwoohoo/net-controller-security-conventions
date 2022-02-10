using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SecurityConventions.UnitTests.Infrastructure
{
    /// <summary>
    /// DataSource to enumerate all controllers in an assembly
    /// </summary>
    public class ControllersDataSource : Attribute, ITestDataSource
    {
        private List<Assembly> Assemblies;

        public ControllersDataSource(Type fromAssemblyContaining)
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
                    var result = new object[1] { controller };

                    yield return result;
                }
            }

            yield break;
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var className = ((Type)data[0]).FullName;
            return className;
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
