using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SecurityConventions.UnitTests.Infrastructure
{
    public abstract class AttributeDataSourceBase : Attribute, ITestDataSource
    {
        private List<Type> Classes;

        public AttributeDataSourceBase(Type forAllAttributesOnClass)
        {
            if (null == forAllAttributesOnClass) throw new System.ArgumentNullException(nameof(forAllAttributesOnClass));

            Classes = new List<Type>() { forAllAttributesOnClass };
        }

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            foreach (var type in Classes)
            {
                var acknowledgeAnonymousControllerAttributes = GetCustomAttributes(type);

                foreach (var a in acknowledgeAnonymousControllerAttributes)
                {
                    var result = new object[1] { a };

                    yield return result;
                }
            }

            yield break;
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            return GetDisplayNameHelper(methodInfo, data);
        }

        protected internal virtual string GetDisplayNameHelper(MethodInfo methodInfo, object[] data)
        {
            var className = ((AcknowledgeAnonymousControllerAttribute)data[0]).Controller.FullName;
            return className;
        }

        protected internal virtual IEnumerable<Attribute> GetCustomAttributes(Type type)
        {
            return type.GetCustomAttributes<AcknowledgeAnonymousControllerAttribute>();
        }
    }
}
