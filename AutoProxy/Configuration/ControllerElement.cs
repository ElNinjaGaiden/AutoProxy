using System.Configuration;

namespace AutoProxy.Configuration
{
    internal class ControllerElement : ConfigurationElement, IController
    {
        [ConfigurationProperty("name", IsRequired = true)]
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
    }
}
