using System;

namespace SecurityConventions.UnitTests.Infrastructure
{
    /// <summary>
    /// Acknowledge an authorized action methods in an anonymous controller. 
    /// 
    /// RATIONALE: By default, all action methods in an anonymous controller are also anonymous and therefore insecure. Secure action methods in an anonymous controller
    /// must be explicitly acknowledged to prevent accidental local development changes making it to production. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AcknowledgeAuthorizedActionMethodAttribute : Attribute
    {
        public AcknowledgeAuthorizedActionMethodAttribute(Type controller, string methodName, string because)
        {
            Controller = controller ?? throw new System.ArgumentNullException(nameof(controller));
            MethodName = methodName ?? throw new System.ArgumentNullException(nameof(methodName));
            Because = because ?? throw new System.ArgumentNullException(nameof(because));
        }

        public readonly Type Controller;
        public readonly string MethodName;
        public readonly string Because;
    }
}
