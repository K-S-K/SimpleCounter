using System.Text;
using Microsoft.AspNetCore.Mvc;
using SimpleCounter.Core;

namespace SimpleCounter.API
{
    public class Program
    {
        private static ICounterCore? counterCore;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton(typeof(ICounterCore), new CounterCore());

            var app = builder.Build();


            counterCore = (ICounterCore)app.Services.GetRequiredService(typeof(ICounterCore));


            app.MapGet("/", () => "SimpleCounter V.0.0.1");

            app.MapGet("/counter/{id}", (HttpContext http, string id) =>
            {
                string mimeType = "image/svg+xml";
                string content = CreateCounterImage(id);

                http.Response.Headers.CacheControl = "no-cache";

                StringBuilder sb = new();
                sb.AppendLine($"{DateTime.UtcNow:yyyy.dd.MM HH:mm:ss} Referers[{http.Request.Headers.Referer.Count}]");
                if (http.Request.Headers.Referer.Count > 0)
                {
                    sb.AppendLine("{");
                    foreach (var referer in http.Request.Headers.Referer)
                    {
                        sb.AppendLine($" {referer}");
                    }
                    sb.AppendLine("}");
                    sb.AppendLine();
                }

                Console.WriteLine(sb.ToString());

                MemoryStream stream = new(Encoding.UTF8.GetBytes(content));
                return Results.File(stream, mimeType, $"{id}.svg");
                // return Results.Text(content, mimeType);
            });

            app.Run();
        }

        /// <summary>
        /// Gets SVG image to return as a result
        /// </summary>
        /// <param name="id">Counter Identifier</param>
        /// <returns>SVG Image Content</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <remarks>
        /// The counter can be requested like this:
        /// 
        /// <html>
        /// <body>
        ///     <img src="https://localhost:7025/counter/863e8de9-6ad7-4f00-84c1-8bacb998e26c" />
        /// </body>
        /// </html>
        /// 
        /// </remarks>
        private static string CreateCounterImage(string id)
        {
            if (counterCore == null)
            {
                throw new NullReferenceException(nameof(counterCore));
            }

            Guid pageId = Guid.Parse(id);
            string content = counterCore.CreateCounterImage(pageId);
            return content;
        }
    }
}