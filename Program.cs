using System;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.Identity;
using Azure.Core;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using find_azure_service_tag;

namespace find_azure_service_tag
{
    class Program
    {
        private static readonly string CONFIGURATION_FILE = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        private static readonly string SECRETS_CONFIGURATION_FILE = Path.Combine(Directory.GetCurrentDirectory(), "appsecrets.json");
        
        private static readonly string SERVICE_TAGS_FILE = Path.Combine(Directory.GetCurrentDirectory(), "servicetags.json");
        
        static void Main(string[] args)
        {
            try
            {
                var config = SetupConfig(args);
                var ip = config["ip"];

                if(string.IsNullOrEmpty(ip))
                {
                    DownloadServiceTags(config["subscriptionid"], config["location"]);
                    return;
                }

                Run(ip);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


       private static IConfiguration SetupConfig(string[] args)
       {
            var builder = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(CONFIGURATION_FILE, false)
                .AddJsonFile(SECRETS_CONFIGURATION_FILE, true)
                .AddCommandLine(args)
                .AddEnvironmentVariables();

            return builder.Build();
       } 

        private static void Run(string ipaddr)
        {
            var target = IPAddress.Parse(ipaddr);
            dynamic tags = JArray.ReadFrom(new JsonTextReader(File.OpenText(SERVICE_TAGS_FILE)));
            foreach(var tag in tags.values)
            {
                var prefix = FindPrefixInServiceTag(tag, target);
                if(!string.IsNullOrEmpty(prefix))
                {
                    Console.WriteLine("{0} is in {1} ({2})", target, prefix, tag.id);
                }
            }
        }

        private static string FindPrefixInServiceTag(JObject serviceTag, IPAddress target)
        {
            dynamic st = serviceTag;
            foreach(var ap in st.properties.addressPrefixes)
            {
                var addrpre = ap.ToString();
                if(IsIn(target, addrpre))
                     return addrpre;
            }
            return null;
        }

        public static bool IsIn(IPAddress ipaddr, string addressPrefix)
        {
            var prefix = IPAddress.Parse(addressPrefix.Split('/')[0]);
            var prefixlength = int.Parse(addressPrefix.Split('/')[1]);
            return IsIn(ipaddr, prefix, prefixlength);
        }
        public static bool IsIn(IPAddress ipaddr, IPAddress prefix, int prefixLength)
        {
            return new Microsoft.AspNetCore.HttpOverrides.IPNetwork(prefix, prefixLength).Contains(ipaddr);
        }

        // https://docs.microsoft.com/ja-jp/rest/api/virtualnetwork/servicetags/list
        // https://docs.microsoft.com/ja-jp/azure/active-directory/develop/msal-acquire-cache-tokens
        private static void DownloadServiceTags(string subscriptionId, string location)
        {
            var cred = new DefaultAzureCredential(true);
            var ctx = new TokenRequestContext( new string[]{"https://management.core.windows.net/.default"} );
            var at = cred.GetToken(ctx);

            using(var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", at.Token);

                var url = $"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.Network/locations/{location}/serviceTags?api-version=2020-07-01";

                Console.WriteLine($"Downloading Service Tags to {SERVICE_TAGS_FILE}　...");               
                var res = client.GetStringAsync(url).Result;
                File.WriteAllText(SERVICE_TAGS_FILE, res);
            }
        }
    }

}
