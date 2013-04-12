using AutoProxy.Configuration;

namespace AutoProxy
{
    public class AutoProxyConfiguration: IAutoProxyConfiguration
    {

        public string Output { get; set; }

        public bool ProxyPerController { get; set; }

        public Library LibraryConfiguration { get; set; }

        public string Namespace { get; set; }

        public ILibrary Library
        {
            get
            {
                return LibraryConfiguration;
            }
        }
    }
}
