using System.Configuration;

namespace AutoProxy.Configuration
{
    internal class FileConfig : ConfigurationElement
    {
        [ConfigurationProperty("src", IsRequired = true)]
        public string Src
        {
            get
            {
                return (string)this["src"];
            }
            set
            {
                this["src"] = value;
            }
        }
    }
}
