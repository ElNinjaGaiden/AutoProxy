using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Http;
using AutoProxy.Configuration;
using Microsoft.Ajax.Utilities;

namespace AutoProxy
{
    /// <summary>
    /// 
    /// </summary>
    public class ProxyGenerator
    {
        public IEnumerable<Assembly> Assemblies { get; private set; }

        public IAutoProxyConfiguration Configuration { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="configuration"></param>
        public ProxyGenerator(IEnumerable<Assembly> assemblies, AutoProxyConfiguration configuration)
        {
            this.Assemblies = assemblies;
            this.Configuration = configuration;
        }

        /// <summary>
        /// Loads from configuration file for "AutoProxy" configuration section
        /// </summary>
        /// <param name="assemblies"></param>
        public ProxyGenerator(IEnumerable<Assembly> assemblies)
        {
            this.Assemblies = assemblies;
            this.Configuration = (AutoProxyConfigurationSection)ConfigurationManager.GetSection("AutoProxy");
        }

        /// <summary>
        /// Project a collection of controllers contained into the given web api assembly.
        /// </summary>
        private IEnumerable<ControllerMetadata> Controllers
        {
            get
            {
                var flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;

                IEnumerable<ControllerMetadata> controllers =
                    this.Assemblies.SelectMany(a => a.GetTypes())
                    .Where(t => t.IsSubclassOf(typeof(ApiController)))
                    .Select(o => new ControllerMetadata
                    {
                        Name = o.Name.Replace("Controller", string.Empty),
                        Actions = o.GetMethods(flags).Select(i => i)
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

            //Iterate over each api controller found
            foreach (var controller in this.Controllers)
            {
                //This creates the prototype definition and make it inherits from the BaseProxy prototype. Example:
                //function MyController (apiAddress) {
                //  BaseProxy.call(this, apiAddress, 'MyController');
                //}
                //inheritPrototype(CoreProxy, BaseProxy);

                string prototype = "function " + controller.Name + "Proxy(apiAddress) { " + Environment.NewLine +
                                    "   BaseProxy.call(this, apiAddress, '" + controller.Name + "'); " + Environment.NewLine +
                                    "} " + Environment.NewLine + Environment.NewLine +
                                    "inheritPrototype(" + controller.Name + "Proxy, BaseProxy);" + Environment.NewLine + Environment.NewLine;

                //Iterate over controller actions in order to add a new function to the prototype for each action found 
                foreach (var action in controller.Actions)
                {
                    var hasParameters = action.GetParameters().Any();

                    prototype += controller.Name + "Proxy.prototype." + action.Name + " = function (" + (hasParameters ? "request, " : string.Empty) + "callback, context, carryover) { " + Environment.NewLine +
                                "   this.ExecuteRequest('" + action.ResolveWebMethodType() + "', '" + action.Name + "', " + (hasParameters ? "request, " : "null, ") + "callback, context, carryover); " + Environment.NewLine +
                                "}; " + Environment.NewLine + Environment.NewLine;
                }

                //Generate each separated proxy (according configuration)
                if (this.Configuration.ProxyPerController)
                {
                    //Save the current prototype into a script file
                    var path = string.Format("{0}/{1}.{2}", this.Configuration.Output, controller.Name + "Proxy", "js");
                    prototype.SaveTo(path);
                    result.Prototypes.Add(new ScriptFile { Src = path, Content = prototype });
                }
            }

            //Generate the all minified proxy (according configuration)
            if (this.Configuration.Minified.Generate)
            {
                //Include required scripts first (according files listed into the "Include" node on the configuration section)
                var requiredFilePaths = this.Configuration.Minified.RequiredFiles.OfType<FileElement>().Select(f =>
                {
                    var path = f.Src;

                    if (path.StartsWith("~"))
                        path = path.Replace("~", HostingEnvironment.ApplicationPhysicalPath);

                    return Path.Combine(this.Configuration.Output, path);
                });

                var all = string.Join(Environment.NewLine, result.Prototypes.Select(p => p.Content));
                var required = string.Join(Environment.NewLine, requiredFilePaths.Select(p => p.ReadFileContent()));
                all = required + all;

                //Minify all content
                var minifier = new Minifier();
                var minified = minifier.MinifyJavaScript(all);

                //Save the minified content into a separated file
                //It uses the given name if exists, otherwise the file name will be "autoproxy-min.js"
                var minifiedName = !string.IsNullOrEmpty(this.Configuration.Minified.Name) ? this.Configuration.Minified.Name : "autoproxy.min";
                var minifiedPath = string.Format("{0}/{1}.{2}", this.Configuration.Output, minifiedName, "js");
                minified.SaveTo(minifiedPath);

                result.Minified = new ScriptFile { Content = minified, Src = minifiedPath };
            }

            return result;
        }
    }
}
