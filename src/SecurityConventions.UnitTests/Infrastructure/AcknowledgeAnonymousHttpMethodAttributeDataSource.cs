using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SecurityConventions.UnitTests.Infrastructure
{
    public class AcknowledgeAnonymousHttpMethodAttributeDataSource : Attribute, ITestDataSource
    {
        private List<Type> Classes;

        public AcknowledgeAnonymousHttpMethodAttributeDataSource(Type forAllAttributesOnClass)
        {
            if (null == forAllAttributesOnClass) throw new System.ArgumentNullException(nameof(forAllAttributesOnClass));

            Classes = new List<Type>() { forAllAttributesOnClass };
        }

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            foreach (var type in Classes)
            {
                var acknowledgeAnonymousHttpMethodAttributes = type.GetCustomAttributes<AcknowledgeAnonymousHttpMethodAttribute>();

                foreach (var a in acknowledgeAnonymousHttpMethodAttributes)
                {
                    var result = new object[1] { a };

                    yield return result;
                }
            }

            yield break;
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var a = ((AcknowledgeAnonymousHttpMethodAttribute)data[0]);
            var fullName = $"{a.Controller.FullName}.{a.MethodName}";
            return fullName;
        }
    }
}
