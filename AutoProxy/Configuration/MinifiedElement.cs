using System.Configuration;
using System.Linq;

namespace AutoProxy.Configuration
{
    internal class MinifiedElement : ConfigurationElement, IMinified
    {
        [ConfigurationProperty("generate", IsRequired = false, DefaultValue = true)]
        public bool Generate
        {
            get
            {
                return (bool)this["generate"];
            }
            set
            {
                this["generate"] = value;
            }
        }

        [ConfigurationProperty("name", IsRequired = false, DefaultValue = "autoproxy.min")]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("Include", IsRequired = false)]
        public IncludeConfigurationCollection Include
        {
            get
            {
                return ((IncludeConfigurationCollection)(base["Include"]));
            }
            set
            {
                base["Include"] = value;
            }
        }


        public System.Collections.Generic.IEnumerable<IFile> RequiredFiles
        {
            get
            {
                return this.Include.OfType<FileElement>();
            }
        }
    }
}
