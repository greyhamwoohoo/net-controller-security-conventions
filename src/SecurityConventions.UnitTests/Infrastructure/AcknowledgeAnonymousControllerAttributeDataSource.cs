using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SecurityConventions.UnitTests.Infrastructure
{
    public class AcknowledgeAnonymousControllerAttributeDataSource : Attribute, ITestDataSource
    {
        private List<Type> Classes;

        public AcknowledgeAnonymousControllerAttributeDataSource(Type forAllAttributesOnClass)
        {
            if (null == forAllAttributesOnClass) throw new System.ArgumentNullException(nameof(forAllAttributesOnClass));

            Classes = new List<Type>() { forAllAttributesOnClass };
        }

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            foreach (var type in Classes)
            {
                var acknowledgeAnonymousControllerAttributes = type.GetCustomAttributes<AcknowledgeAnonymousControllerAttribute>();

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
            var className = ((AcknowledgeAnonymousControllerAttribute)data[0]).Controller.FullName;
            return className;
        }
    }
}
