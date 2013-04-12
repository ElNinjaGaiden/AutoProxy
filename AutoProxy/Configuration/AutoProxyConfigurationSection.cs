using System.Configuration;

namespace AutoProxy.Configuration
{
    internal class AutoProxyConfigurationSection : ConfigurationSection, IAutoProxyConfiguration
    {
        [ConfigurationProperty("Library", IsRequired = false)]
        public LibraryElement LibraryConfiguration
        {
            get
            {
                return (LibraryElement)this["Library"];
            }
            set
            {
                this["Library"] = value;
            }
        }


        public ILibrary Library
        {
            get
            {
                return this.LibraryConfiguration;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
