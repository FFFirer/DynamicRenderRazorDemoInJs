using System.Text.Json.Serialization;

namespace DynamicRazorRender.Server.Controllers
{
    public class RenderPlainRequest
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}