using System.Configuration;

namespace AutoProxy.Configuration
{
    internal class AutoProxyConfigurationSection : ConfigurationSection
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

        [ConfigurationProperty("generateMinified", IsRequired = false, DefaultValue = true)]
        public bool GenerateMinified
        {
            get
            {
                return (bool)this["generateMinified"];
            }
            set
            {
                this["generateMinified"] = value;
            }
        }

        [ConfigurationProperty("minifiedName", IsRequired = false)]
        public string MinifiedName
        {
            get
            {
                return (string)this["minifiedName"];
            }
            set
            {
                this["minifiedName"] = value;
            }
        }

        [ConfigurationProperty("generateEach", IsRequired = false, DefaultValue = true)]
        public bool GenerateEach
        {
            get
            {
                return (bool)this["generateEach"];
            }
            set
            {
                this["generateEach"] = value;
            }
        }

        [ConfigurationProperty("Include", IsRequired = false)]
        public IncludeConfigurationCollection RequiredFiles
        {
            get { return ((IncludeConfigurationCollection)(base["Include"])); }
            set { base["Include"] = value; }
        }
    }
}
