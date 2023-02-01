using System.Text.Json.Serialization;

namespace DynamicRazorRender.Server.Controllers
{
    public class RenderFileRequest
    {
        [JsonPropertyName("filepath")]
        public string? FilePath {get;set;}
    }
}