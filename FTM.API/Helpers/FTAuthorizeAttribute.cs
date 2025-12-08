using FTM.Domain.Enums;

namespace FTM.API.Helpers
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class FTAuthorizeAttribute : Attribute
    {
        public MethodType Method { get; }
        public FeatureType Feature { get; }

        public FTAuthorizeAttribute(MethodType method, FeatureType feature)
        {
            Method = method;
            Feature = feature;
        }
    }
}
