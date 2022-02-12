using System;

namespace SecurityConventions.UnitTests.Infrastructure
{
    /// <summary>
    /// Acknowledge an authorized method in an anonymous controller. 
    /// 
    /// RATIONALE: By default, all HttpMethods in an anonymous controller are also anonymous and therefore insecure. Secure HttpMethods in an anonymous controller
    /// must be explicitly acknowledged to prevent accidental local development changes making it to production. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AcknowledgeAuthorizedHttpMethodAttribute : Attribute
    {
        public AcknowledgeAuthorizedHttpMethodAttribute(Type controller, string methodName, string because)
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
