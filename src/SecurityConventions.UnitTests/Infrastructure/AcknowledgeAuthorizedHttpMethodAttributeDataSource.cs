using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SecurityConventions.UnitTests.Infrastructure
{
    public class AcknowledgeAuthorizedHttpMethodAttributeDataSource : Attribute, ITestDataSource
    {
        private List<Type> Classes;

        public AcknowledgeAuthorizedHttpMethodAttributeDataSource(Type forAllAttributesOnClass)
        {
            if (null == forAllAttributesOnClass) throw new System.ArgumentNullException(nameof(forAllAttributesOnClass));

            Classes = new List<Type>() { forAllAttributesOnClass };
        }

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            foreach (var type in Classes)
            {
                var acknowledgeAuthorizedHttpMethodAttributes = type.GetCustomAttributes<AcknowledgeAuthorizedHttpMethodAttribute>();

                foreach (var a in acknowledgeAuthorizedHttpMethodAttributes)
                {
                    var result = new object[1] { a };

                    yield return result;
                }
            }

            yield break;
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var a = ((AcknowledgeAuthorizedHttpMethodAttribute)data[0]);
            var fullName = $"{a.Controller.FullName}.{a.MethodName}";
            return fullName;
        }
    }
}
