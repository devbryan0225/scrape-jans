using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WaterDisruptionFunctionApp
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var result = await GetJANSAsync();
            log.LogInformation(result);
            var titles = Parse(result);

            /**
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
            **/
            return new OkObjectResult(titles);
        }

        private static async Task<string> GetJANSAsync()
        {
            HttpClient client = new HttpClient();
            //var response = await client.GetStringAsync("https://water.sabah.gov.my/rss-feed.xml");
            var response = await client.GetStringAsync("https://water.sabah.gov.my/notice");
            return response;

        }

        private static void ParseRSS(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var posts = doc.DocumentNode.Descendants("item");
            foreach (var post in posts)
            {
                var title = post.Descendants("title").FirstOrDefault().FirstChild;
                Console.WriteLine(title);
                //var body = title.FirstOrDefault().Descendants("body").FirstOrDefault();
            }
            //var posts = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("data-ad-preview", "").Contains("message")).ToList();

        }

        private static List<string> Parse(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var posts = doc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("update-body"));
            List<string> results = new List<string>();
            foreach (var post in posts)
            {
                var title = post.Descendants("h2").FirstOrDefault().InnerText;
                results.Add(title);
                
                //var body = title.FirstOrDefault().Descendants("body").FirstOrDefault();
            }
            return results;

        }
    }
}
