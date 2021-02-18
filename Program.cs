using System;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace find_azure_service_tag
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Run(args[0]);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void Run(string ipaddr)
        {
            var target = IPAddress.Parse(ipaddr);
            var servicetags = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "servicetags.json");
            dynamic tags = JArray.ReadFrom(new JsonTextReader(File.OpenText(servicetags)));
            foreach(var tag in tags)
            {
                var prefix = FindPrefixInServiceTag(tag, target);
                if(!string.IsNullOrEmpty(prefix))
                {
                    Console.WriteLine("{0} is in {1} ({2})", target, prefix, tag.Id);
                }
            }
        }

        private static string FindPrefixInServiceTag(JObject serviceTag, IPAddress target)
        {
            dynamic st = serviceTag;
            string addressPrefixes = st.Properties.AddressPrefixes.ToString();
            var prefix = addressPrefixes.Split(" ").Where( ap => target.IsIn(ap)).FirstOrDefault();
            return prefix;
        }
    }

    public static class Helper
    {
        public static bool IsIn(this IPAddress ipaddr, string addressPrefix)
        {
            var prefix = IPAddress.Parse(addressPrefix.Split('/')[0]);
            var prefixlength = int.Parse(addressPrefix.Split('/')[1]);
            return ipaddr.IsIn(prefix, prefixlength);
        }
        public static bool IsIn(this IPAddress ipaddr, IPAddress prefix, int prefixLength)
        {
            return new Microsoft.AspNetCore.HttpOverrides.IPNetwork(prefix, prefixLength).Contains(ipaddr);
        }
    }
}
