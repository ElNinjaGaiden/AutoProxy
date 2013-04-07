namespace AutoProxy
{
    public interface IAutoProxyConfiguration
    {
        string Output { get; }

        bool ProxyPerController { get; }

        IMinified Minified { get; }
    }
}
