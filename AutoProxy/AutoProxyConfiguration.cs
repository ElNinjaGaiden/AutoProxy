using AutoProxy.Configuration;

namespace AutoProxy
{
    public class AutoProxyConfiguration: IAutoProxyConfiguration
    {
        public Library LibraryConfiguration { get; set; }

        public ILibrary Library
        {
            get
            {
                return LibraryConfiguration;
            }
        }
    }
}
