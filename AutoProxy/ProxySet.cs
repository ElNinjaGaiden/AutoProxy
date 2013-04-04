using System.Collections.Generic;

namespace AutoProxy
{
    public class ProxySet
    {
        public List<ScriptFile> Prototypes { get; set; }

        public ScriptFile Minified { get; set; }

        public ProxySet()
        {
            this.Prototypes = new List<ScriptFile>();
        }
    }
}
