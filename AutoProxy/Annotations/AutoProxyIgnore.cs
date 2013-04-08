using System;

namespace AutoProxy.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AutoProxyIgnore : Attribute
    {
    }
}
