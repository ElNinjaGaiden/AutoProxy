using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Web.Http;
using AutoProxy.Annotations;
using AutoProxy.Configuration;
using Microsoft.Ajax.Utilities;

namespace AutoProxy
{
    /// <summary>
    /// 
    /// </summary>
    public class ProxyGenerator
    {
        public IAutoProxyConfiguration Configuration { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="configuration"></param>
        public ProxyGenerator(AutoProxyConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Loads from configuration file for "AutoProxy" configuration section
        /// </summary>
        /// <param name="assemblies"></param>
        public ProxyGenerator()
        {
            this.Configuration = (AutoProxyConfigurationSection)ConfigurationManager.GetSection("AutoProxy");
        }

        /// <summary>
        /// Project a collection of controllers
        /// </summary>
        private IEnumerable<ControllerMetadata> Controllers
        {
            get
            {
                var flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;

                IEnumerable<ControllerMetadata> controllers = null;
                Func<Type, bool> controllersfilter = null;

                if (this.Configuration.Library.Controllers != null && this.Configuration.Library.Controllers.Any())
                {
                    controllersfilter = t => t.IsSubclassOf(typeof(ApiController)) 
                                    && this.Configuration.Library.Controllers.Any(c => c.Name.Equals(t.Name, StringComparison.InvariantCultureIgnoreCase));
                }
                else
                {
                    controllersfilter = t => t.IsSubclassOf(typeof(ApiController)) && t.GetCustomAttribute<AutoProxyIgnore>() == null;
                }

                controllers = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                    .Where(controllersfilter)
                    .Select(o => new ControllerMetadata
                    {
                        Name = o.Name.Replace("Controller", string.Empty),
                        Actions = o.GetMethods(flags).Where(a => a.GetCustomAttribute<AutoProxyIgnore>() == null).Select(i => i),
                        ProxyName = o.GetProxyName(o.Name.Replace("Controller", "Proxy"))
                    });

                return controllers;
            }
        }

        /// <summary>
        /// Generates javascript proxies for web api controllers.
        /// Proxies will be stored on the configured location.
        /// This creates one proxy file class per controller and/or one minified file containing all the proxies. 
        /// </summary>
        /// <returns></returns>
        public ProxySet ResolveProxies()
        {
            ProxySet result = new ProxySet();
            var controllers = this.Controllers;

            if (controllers.Any())
            {
                //Include required scripts first (according files listed into the "Include" node on the configuration section)
                var requiredFilePaths = this.Configuration.Library.IncludeFiles.Select(f =>
                {
                    var path = f.Src;

                    if (path.StartsWith("~"))
                        path = path.Replace("~", HostingEnvironment.ApplicationPhysicalPath);

                    return path;
                });

                var required = string.Join(Environment.NewLine + Environment.NewLine, requiredFilePaths.Select(p => p.ReadFileContent()));
                var content = string.Empty;

                //Iterate over each api controller found
                foreach (var controller in controllers)
                {
                    //This creates the prototype definition and make it inherits from the BaseProxy prototype. Example:
                    string prototype = "function " + controller.ProxyName + "() { " + Environment.NewLine +
                                        "   __namespace__BaseProxy.call(this, '" + controller.Name + "'); " + Environment.NewLine +
                                        "} " + Environment.NewLine + Environment.NewLine +
                                        "inheritPrototype(" + controller.ProxyName + ", __namespace__BaseProxy);" + Environment.NewLine + Environment.NewLine;

                    //Iterate over controller actions in order to add a new function to the prototype for each action found 
                    foreach (var action in controller.Actions)
                    {
                        var hasParameters = action.GetParameters().Any();

                        prototype += controller.ProxyName + ".prototype." + action.GetProxyName(action.Name) + " = function (" + (hasParameters ? "request, " : string.Empty) + "context) { " + Environment.NewLine +
                                    "   return this.ExecReq('" + action.ResolveWebMethodType() + "', '" + action.Name + "', " + (hasParameters ? "request, " : "null, ") + "context); " + Environment.NewLine +
                                    "}; " + Environment.NewLine + Environment.NewLine;
                    }

                    content += Environment.NewLine + prototype;
                }

                //Replace the namespace
                Regex rgx = new Regex("__namespace__");
                content = rgx.Replace(string.Concat(required, Environment.NewLine, content), this.Configuration.Library.Namespace);

                if (this.Configuration.Library.Compress)
                {
                    //Minify all content
                    var minifier = new Minifier();
                    content = minifier.MinifyJavaScript(content);
                }

                var filePath = string.Empty;
                if (this.Configuration.Library.SaveFile) 
                {
                    //File path, by default "~/Scripts/proxy/autoproxy.min.js"
                    filePath = string.IsNullOrEmpty(this.Configuration.Library.Output) ? "~/Scripts/proxy/autoproxy.min.js" : this.Configuration.Library.Output;
                    content.SaveTo(filePath);
                }

                result.Library = new ScriptFile { Content = content, Src = filePath };
            }

            return result;
        }
    }
}
