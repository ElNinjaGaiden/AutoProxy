using System.Configuration;

namespace AutoProxy.Configuration
{
    internal class AutoProxyConfigurationSection : ConfigurationSection, IAutoProxyConfiguration
    {
        [ConfigurationProperty("output", IsRequired = true)]
        public string Output
        {
            get
            {
                return (string)this["output"];
            }
            set
            {
                this["output"] = value;
            }
        }

        [ConfigurationProperty("proxyPerController", IsRequired = false, DefaultValue = true)]
        public bool ProxyPerController
        {
            get
            {
                return (bool)this["proxyPerController"];
            }
            set
            {
                this["proxyPerController"] = value;
            }
        }

        [ConfigurationProperty("namespace", IsRequired = true)]
        public string Namespace
        {
            get
            {
                return (string)this["namespace"];
            }
            set
            {
                this["namespace"] = value;
            }
        }

        [ConfigurationProperty("Minified", IsRequired = false)]
        public MinifiedElement MinifiedConfiguration
        {
            get
            {
                return (MinifiedElement)this["Minified"];
            }
            set
            {
                this["Minified"] = value;
            }
        }


        public IMinified Minified
        {
            get
            {
                return this.MinifiedConfiguration;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
