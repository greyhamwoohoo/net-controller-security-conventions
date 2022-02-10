using System;

namespace SecurityConventions.UnitTests.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AcknowledgeAnonymousControllerAttribute : Attribute
    {
        public AcknowledgeAnonymousControllerAttribute()
        {
        }

        public Type Controller { get; set; }
    }
}
