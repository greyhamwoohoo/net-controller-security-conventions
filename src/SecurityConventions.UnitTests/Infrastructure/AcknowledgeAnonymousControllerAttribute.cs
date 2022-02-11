using System;

namespace SecurityConventions.UnitTests.Infrastructure
{
    /// <summary>
    /// Acknowledge that a Controller is marked as [AllowAnonymous]
    /// This provides a hint that the Controller and all of its nested methods are implicitly insecure by default. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AcknowledgeAnonymousControllerAttribute : Attribute
    {
        public AcknowledgeAnonymousControllerAttribute()
        {
        }

        public Type Controller { get; set; }
    }
}
