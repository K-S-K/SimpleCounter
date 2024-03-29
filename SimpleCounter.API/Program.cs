using System.Text;
using Microsoft.AspNetCore.HttpLogging;

using SimpleCounter.Core;
using SimpleCounter.Mongo;
using SimpleCounter.Common;

namespace SimpleCounter.API
{
    public class Program
    {
        /// <summary>
        /// Program entry point
        /// 
        /// How to publish
        /// dotnet publish -c Release
        /// 
        /// Documentation on project
        /// https://github.com/K-S-K/SimpleCounter
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Get counters database configuration
            builder.Services.Configure<CountersDatabaseSettings>(
                builder.Configuration.GetSection("CountersDatabase"));

            // Register counters storage
            builder.Services.AddSingleton<ICounterStorage, CounterStorage>();

            // Register counters infrastructure
            builder.Services.AddCountersInfrastructure();

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


            app.MapGet("/", () => "SimpleCounter V.0.0.1");

            app.MapGet("/counters/", ListCounters);

            app.Map("/counter/{id}", CreateCounterImage);

            app.Run();
        }

        /// <summary>
        /// Gets SVG image to return as a result
        /// </summary>
        /// <param name="context">Http context</param>
        /// <param name="id">Counter Identifier</param>
        /// <param name="counterCore">The Counter Processor Service</param>
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
        private static IResult CreateCounterImage(HttpContext context, string id, ICounterCore counterCore)
        {
            ReqRport(context);

            string content;
            try
            {
                if (counterCore == null)
                {
                    throw new NullReferenceException(nameof(counterCore));
                }

                Guid pageId = Guid.Parse(id);
                content = counterCore.CreateCounterImage(pageId);
            }
            catch (Exception ex)
            {
                var errors
                = new Dictionary<string, string[]>
                { { id, new[] { ex.Message } } };

                return Results.ValidationProblem(errors);
            }

            context.Response.Headers.CacheControl = "no-cache";

            string mimeType = "image/svg+xml";
            MemoryStream stream = new(Encoding.UTF8.GetBytes(content));
            return Results.File(stream, mimeType, $"{id}.svg");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">The Http Request Context</param>
        /// <param name="counterData">The Counter Data Service</param>
        /// <returns></returns>
        private static IResult ListCounters(HttpContext context, ICounterData counterData)
        {
            ReqRport(context);
            return Results.Ok(counterData.Counters);
        }

        /// <summary>
        /// Creates the request report for the log
        /// </summary>
        /// <param name="context">The Http Request Context</param>
        private static void ReqRport(HttpContext context)
        {
            StringBuilder sb = new();
            sb.AppendLine($"{DateTime.UtcNow:yyyy.dd.MM HH:mm:ss} Referers[{context.Request.Headers.Referer.Count}]");
            if (context.Request.Headers.Referer.Count > 0)
            {
                sb.AppendLine("{");
                foreach (var referer in context.Request.Headers.Referer)
                {
                    sb.AppendLine($" {referer}");
                }
                sb.AppendLine("}");
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
        }
    }
}