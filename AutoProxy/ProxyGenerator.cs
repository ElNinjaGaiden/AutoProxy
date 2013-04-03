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
        private Assembly ApiAssembly { get; set; }

        public ProxyGenerator(Assembly assembly)
        {
            this.ApiAssembly = assembly;
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
                    this.ApiAssembly.GetTypes()
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
        public void ResolveProxies()
        {
            var configuration = (AutoProxyConfigurationSection)ConfigurationManager.GetSection("AutoProxy");

            if (configuration != null)
            {
                var all = string.Empty;

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
                    if (configuration.GenerateEach)
                    {
                        //Save the current prototype into a script file
                        var path = string.Format("{0}/{1}.{2}", configuration.Output, controller.Name + "Proxy", "js");
                        prototype.SaveTo(path);
                    }

                    //Append the current prototype to the minified version
                    if (configuration.GenerateMinified)
                        all += prototype;
                }

                //Generate the all minified proxy (according configuration)
                if (configuration.GenerateMinified)
                {
                    //Include required scripts first (according files listed into the "Include" node on the configuration section)
                    var requiredFilePaths = configuration.RequiredFiles.OfType<FileConfig>().Select(f =>
                    {
                        var path = f.Src;

                        if (path.StartsWith("~"))
                            path = path.Replace("~", HostingEnvironment.ApplicationPhysicalPath);

                        return Path.Combine(configuration.Output, path);
                    });

                    var required = string.Join(Environment.NewLine, requiredFilePaths.Select(p => p.ReadFileContent()));
                    all = required + all;

                    //Minify the all content
                    var minifier = new Minifier();
                    var minified = minifier.MinifyJavaScript(all);

                    //Save the minified content into a file
                    //It uses the given name if exists, otherwise the file name will be "autoproxy-min.js"
                    var minifiedName = !string.IsNullOrEmpty(configuration.MinifiedName) ? configuration.MinifiedName : "autoproxy.min";
                    var minifiedPath = string.Format("{0}/{1}.{2}", configuration.Output, minifiedName, "js");
                    minified.SaveTo(minifiedPath);
                }
            }
        }
    }
}
