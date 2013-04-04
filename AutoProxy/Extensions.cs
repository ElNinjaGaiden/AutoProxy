using System.IO;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Http;

namespace AutoProxy
{
    internal static class Extensions
    {
        public static Stream ToStream(this string str)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static void SaveTo(this string str, string path)
        {
            using (var input = str.ToStream())
            {
                input.SaveTo(path);
            }
        }

        public static void SaveTo(this Stream str, string path)
        {
            if (path.StartsWith("~"))
                path = path.Replace("~", HostingEnvironment.ApplicationPhysicalPath);

            using (FileStream output = System.IO.File.OpenWrite(path))
            {
                str.CopyTo(output);
            }
        }

        public static string ReadFileContent(this string path)
        {
            string content = string.Empty;

            if (System.IO.File.Exists(path))
            {
                using (var input = new StreamReader(path))
                {
                    content = input.ReadToEnd();
                }
            }

            return content;
        }

        public static string ResolveWebMethodType(this MethodInfo methodInfo)
        {
            string method = "GET";

            if (methodInfo.GetCustomAttribute<HttpPostAttribute>() != null)
            {
                method = "POST";
            }
            else
            {
                if (methodInfo.GetCustomAttribute<HttpPutAttribute>() != null)
                {
                    method = "PUT";
                }
                else
                {
                    if (methodInfo.GetCustomAttribute<HttpDeleteAttribute>() != null)
                    {
                        method = "DELETE";
                    }
                }
            }

            return method;
        }
    }
}
