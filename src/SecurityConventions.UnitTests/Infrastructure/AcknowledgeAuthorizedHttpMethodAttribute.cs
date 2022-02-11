using System;

namespace SecurityConventions.UnitTests.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AcknowledgeAuthorizedHttpMethodAttribute : Attribute
    {
        public AcknowledgeAuthorizedHttpMethodAttribute()
        {
        }

        public Type Controller { get; set; }
        public string MethodName { get; set; }
    }
}
