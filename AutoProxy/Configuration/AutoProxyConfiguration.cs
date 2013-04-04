
namespace AutoProxy.Configuration
{
    public class AutoProxyConfiguration: IAutoProxyConfiguration
    {

        public string Output { get; set; }

        public bool ProxyPerController { get; set; }

        public Minified MinifiedConfiguration { get; set; }

        public IMinified Minified
        {
            get
            {
                return MinifiedConfiguration;
            }
        }
    }
}
