using System.Text.Json.Serialization;

namespace DynamicRazorRender.Server.Controllers
{
    public class RenderPlainRequest
    {
        [JsonPropertyName("plainText")]
        public string? PlainText { get; set; }
    }
}