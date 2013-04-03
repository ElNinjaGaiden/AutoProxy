using System.Configuration;

namespace AutoProxy.Configuration
{
    internal class IncludeConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FileConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FileConfig)element).Src;
        }

        protected override string ElementName
        {
            get
            {
                return "File";
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
