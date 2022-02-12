using System;

namespace SecurityConventions.UnitTests.Infrastructure
{
    /// <summary>
    /// Acknowledge that a HttpMethod is marked as [AllowAnonymous]
    /// This provides a hint that the HttpMethod is insecure.  
    /// This is intended to be used to explicitly acknowledge insecure methods in controllers that have the [Authorize] attribute. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AcknowledgeAnonymousHttpMethodAttribute : Attribute
    {
        public AcknowledgeAnonymousHttpMethodAttribute()
        {
        }

        public Type Controller { get; set; }
        public string MethodName { get; set; }

        public override string ToString()
        {
            return $"{Controller?.FullName}.{MethodName}";
        }
    }
}
