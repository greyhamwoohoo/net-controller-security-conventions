using System;

namespace SecurityConventions.UnitTests.Infrastructure
{
    /// <summary>
    /// Acknowledge that a HttpMethod is marked as [Authorize]
    /// This provides a hint that the HttpMethod is secure.  
    /// This is intended to be used to explicitly acknowledge secure methods in controllers that have the [AllowAnonymous] attribute. 
    /// </summary>
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
