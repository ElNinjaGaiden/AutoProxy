using System.Collections.Generic;
using System.Reflection;
using AutoProxy.Annotations;

namespace AutoProxy
{
    internal class ControllerMetadata
    {
        public string Name { get; set; }

        public IEnumerable<MethodInfo> Actions { get; set; }

        public string ProxyName { get; set; }
    }
}
