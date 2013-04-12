using System.Configuration;

namespace AutoProxy.Configuration
{
    internal class ControllersConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ControllerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ControllerElement)element).Name;
        }

        protected override string ElementName
        {
            get
            {
                return "Controller";
            }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }
    }
}
