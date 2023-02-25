using System.Text;

namespace SimpleCounter.API
{
    public class Program
    {
        public static int count = 0;

        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/", () => "SimpleCounter V.0.0.1");

            // 863e8de9-6ad7-4f00-84c1-8bacb998e26c
            app.MapGet("/counter/{id}", () =>
            {
                // var mimeType = "image/png";
                // var path = @"D:\fig.png";
                // return Results.File(path, contentType: mimeType);

                string mimeType = "image/svg+xml";
                string content = TakeTheNumber();

                // return Results.Text(content, mimeType);

                MemoryStream ms = new (Encoding.UTF8.GetBytes(content));
                return Results.File(ms, mimeType, "a.svg");
            });

            app.Run();
        }

        public static string TakeTheNumber()
        {
            StringBuilder sb = new ();

            sb.AppendLine($"<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\" ?>");
            sb.AppendLine($"<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">");
            sb.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" version=\"1.1\" width=\"80\" height=\"24\" viewBox=\"0 0 80 24\" xml:space=\"preserve\">");
            sb.AppendLine($"<rect x=\"0\" y=\"0\" width=\"78\" height=\"20\" stroke=\"black\" stroke-width=\"2\" fill=\"none\"/>");
            sb.AppendLine($"<text x=\"2\" y=\"16\" font-size=\"18\" text-anchor=\"right\" font-family=\"'Open Sans', sans-serif\" font-style=\"italic\" font-weight=\"bold\" style=\"fill: rgb(18,101,18); fill-opacity: 0.75;\">{++count}</text>");
            sb.AppendLine($"</svg>");
            sb.AppendLine($"");
            sb.AppendLine($"");

            string svgContent = sb.ToString();

            return svgContent;
        }
    }
}