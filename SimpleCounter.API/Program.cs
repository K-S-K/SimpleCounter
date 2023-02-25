using System.Text;
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

            app.MapGet("/counter/{id}", (string id) =>
            {
                string mimeType = "image/svg+xml";
                string content = CreateCounterImage(id);

                MemoryStream steram = new(Encoding.UTF8.GetBytes(content));
                return Results.File(steram, mimeType, $"{id}.svg");
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