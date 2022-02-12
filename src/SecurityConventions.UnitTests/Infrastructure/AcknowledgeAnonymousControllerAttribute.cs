using System;

namespace SecurityConventions.UnitTests.Infrastructure
{
    /// <summary>
    /// Acknowledge an anonymous controller. 
    /// 
    /// RATIONALE: Controllers marked as [AllowAnonymous] are insecure. We require an explicit acknowledgement that the controller is intended to be anonymous to prevent
    /// accidental local development changes making it to production. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AcknowledgeAnonymousControllerAttribute : Attribute
    {
        public AcknowledgeAnonymousControllerAttribute(Type controller, string because)
        {
            Controller = controller ?? throw new System.ArgumentNullException(nameof(controller));
            Because = because ?? throw new System.ArgumentNullException(nameof(because));
        }

        public readonly Type Controller;
        public readonly string Because;
    }
}
