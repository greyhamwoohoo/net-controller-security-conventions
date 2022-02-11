using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SecurityConventions.UnitTests.Infrastructure
{
    /// <summary>
    /// DataSource to yield every Controller in a given assembly. 
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
        /// Override in a derived class to select controllers required for this data source. 
        /// </summary>
        /// <param name="controllerType">The Controller type. </param>
        /// <returns>true if the Controller is to be returned by the DataSource; false otherwise. </returns>
        public virtual bool FilterControllerByAttribute(Type controllerType)
        {
            return true;
        }
    }
}
