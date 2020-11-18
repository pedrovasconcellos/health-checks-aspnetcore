using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Vasconcellos.HealthChecks.Core.CoreResources
{
    public static class HealthChecksResponseWriter
    {
        public static Task WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream, GetJsonWriterOptions()))
            {
                writer.WriteStartObject();
                writer.WriteString("status", result.Status.ToString());
                writer.WriteStartObject("components");
                foreach (var entry in result.Entries)
                {
                    writer.WriteString("name", entry.Key);
                    writer.WriteString("status", entry.Value.Status.ToString());
                    writer.WriteString("error", entry.Value.Status !=  HealthStatus.Healthy
                        ? entry.Value.Exception?.Message ?? entry.Value.Description
                        : entry.Value.Exception?.Message);
                }
                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            var json = Encoding.UTF8.GetString(stream.ToArray());
            return context.Response.WriteAsync(json);
        }

        private static JsonWriterOptions GetJsonWriterOptions() => new JsonWriterOptions
        {
            Indented = true
        };
    }
}
