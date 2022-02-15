using System;

namespace SecurityConventions.UnitTests.Infrastructure
{
    /// <summary>
    /// Acknowledge an anonymous action method in an authorized controller. 
    /// 
    /// RATIONALE: By default, all action methods in an authorized controller are also authorized and therefore secure. Insecure action methods in an authorized controller
    /// must be explicitly acknowledged to prevent accidental local development changes making it to production. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AcknowledgeAnonymousActionMethodAttribute : Attribute
    {
        public AcknowledgeAnonymousActionMethodAttribute(Type controller, string methodName, string because)
        {
            Controller = controller ?? throw new System.ArgumentNullException(nameof(controller));
            MethodName = methodName ?? throw new System.ArgumentNullException(nameof(methodName));
            Because = because ?? throw new System.ArgumentNullException(nameof(because));
        }

        public readonly Type Controller;
        public readonly string MethodName;
        public readonly string Because;

        public override string ToString()
        {
            return $"{Controller?.FullName}.{MethodName}";
        }
    }
}
