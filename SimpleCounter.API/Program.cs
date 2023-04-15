using System.Text;
using Microsoft.AspNetCore.HttpLogging;

using SimpleCounter.Core;
using SimpleCounter.Data;
using SimpleCounter.Draw;
using SimpleCounter.Mongo;
using SimpleCounter.Common;

namespace SimpleCounter.API
{
    public class Program
    {
        private static ICounterCore? counterCore;

        /// <summary>
        /// Program entry point
        /// 
        /// How to publish
        /// dotnet publish -c Release
        /// 
        /// Documentation on project
        /// https://github.com/K-S-K/SimpleCounter
        /// 
        /// Documentation on Minimal APIs:
        /// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-7.0
        /// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-7.0
        /// 
        /// Cache control theory
        /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cache-Control
        /// 
        /// Documentation on GitHub's image caching
        /// https://github.com/orgs/community/discussions/22283
        /// https://github.com/community/community/discussions/11884
        /// 
        /// Documentation on image caching
        /// https://stackoverflow.com/questions/728616/disable-cache-for-some-images
        /// 
        /// Documentation on UrlReferrer Property
        /// https://learn.microsoft.com/en-us/dotnet/api/system.web.httprequest.urlreferrer?redirectedfrom=MSDN&view=netframework-4.8.1
        /// 
        /// Mongo
        /// https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-7.0&tabs=visual-studio
        /// https://www.mongodb.com/docs/manual/tutorial/install-mongodb-on-windows/
        /// https://www.mongodb.com/docs/mongodb-shell/install/
        /// 
        /// The book examples:
        /// https://github.com/andrewlock/asp-dot-net-core-in-action-3e
        /// 
        /// Mongo Connect example:
        /// https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app
        /// https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api
        /// 
        /// Mongo Script example:
        /// https://www.mongodb.com/docs/v5.0/tutorial/write-scripts-for-the-mongo-shell/
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<CountersDatabaseSettings>(
                builder.Configuration.GetSection("CountersDatabase"));

            builder.Services.AddSingleton<ICounterStorage, CounterStorage>();
            builder.Services.AddSingleton<IPageCounters, PageCounters>();
            builder.Services.AddSingleton<ICounterDraw, CounterDraw>();
            builder.Services.AddSingleton<ICounterData, CounterData>();
            builder.Services.AddSingleton<ICounterCore, CounterCore>();

            builder.Services.AddHttpLogging(opts => opts.LoggingFields = HttpLoggingFields.RequestProperties);
            builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Debug);
            builder.Services.AddProblemDetails();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseHttpLogging();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.MapGet("/error", () => "An error occurred, but we are already working on it");
            }

            app.MapGet("/e", () => { throw new Exception("Test Exception"); });

            app.Use(async (context, next) =>
            {
                try
                {
                    await next.Invoke();
                }
                catch (Exception ex)
                {
                    //var loggingService = context.RequestServices.GetService<ILoggingService>();
                    //loggingService.Error(ex);
                    Console.Error.WriteLine(ex.Message + " My " + ex.StackTrace);
                    throw;
                }

            });


            counterCore = (ICounterCore)app.Services.GetRequiredService(typeof(ICounterCore));
            var counterData = (ICounterData)app.Services.GetRequiredService(typeof(ICounterData));


            app.MapGet("/", () => "SimpleCounter V.0.0.1");

            app.MapGet("/counters/", (HttpContext http) =>
            {
                ReqRport(http);

                return Results.Ok(counterData.Counters);
            });

            app.MapGet("/counter/{id}", (HttpContext http, string id) =>
            {
                ReqRport(http);

                string mimeType = "image/svg+xml";
                string content;
                try { content = CreateCounterImage(id); }
                catch (Exception ex)
                {
                    var errors
                    = new Dictionary<string, string[]>
                    { { id, new[] { ex.Message } } };

                    return Results.ValidationProblem(errors);
                }

                http.Response.Headers.CacheControl = "no-cache";

                MemoryStream stream = new(Encoding.UTF8.GetBytes(content));
                return Results.File(stream, mimeType, $"{id}.svg");
                // return Results.Text(content, mimeType);
            });

            app.Run();
        }

        private static void ReqRport(HttpContext http)
        {
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