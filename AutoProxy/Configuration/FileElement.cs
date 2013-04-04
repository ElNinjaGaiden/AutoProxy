using System.Configuration;

namespace AutoProxy.Configuration
{
    internal class FileElement : ConfigurationElement, IFile
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
