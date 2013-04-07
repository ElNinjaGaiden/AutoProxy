using System;

namespace AutoProxy.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AutoProxyAlias : Attribute
    {
        public string Value { get; set; }

        public AutoProxyAlias(string value)
        {
            this.Value = value;
        }
    }
}
