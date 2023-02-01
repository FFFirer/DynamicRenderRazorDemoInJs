using System.Text.Json.Serialization;

namespace DynamicRazorRender.Server.Controllers
{
    public class RenderRequest
    {
        public string? RenderType { get; set; }
        public string? Content { get; set; }
    }
}