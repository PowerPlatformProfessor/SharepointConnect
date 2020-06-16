using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SharePointPlugin
{
    public class Account : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = (IOrganizationService)factory.CreateOrganizationService(context.UserId);
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.MessageName != "Create")
                return;

            
            
        }

        private void GetAccessToken()
        {
            try
            {
                X509Certificate2 clientCert = new X509Certificate2(Properties.Resources.Sharepoint, "1234");
                WebRequestHandler requestHandler = new WebRequestHandler();
                requestHandler.ClientCertificates.Add(clientCert);

                HttpClient client = new HttpClient(requestHandler)
                {
                    BaseAddress = new Uri("http://localhost:3020/")
                };

                HttpResponseMessage response = client.GetAsync("customers").Result;
                response.EnsureSuccessStatusCode();
                string responseContent = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(responseContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while executing the test code: {0}", ex.Message);
            }
        }
    }
}
