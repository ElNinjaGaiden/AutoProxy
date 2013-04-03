using System.Collections.Generic;
using System.Reflection;

namespace AutoProxy
{
    internal class ControllerMetadata
    {
        public string Name { get; set; }

        public IEnumerable<MethodInfo> Actions { get; set; }
    }
}
