using System;

namespace SecurityConventions.UnitTests.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AcknowledgeAnonymousHttpMethodAttribute : Attribute
    {
        public AcknowledgeAnonymousHttpMethodAttribute()
        {
        }

        public Type Controller { get; set; }
        public string MethodName { get; set; }
    }
}
