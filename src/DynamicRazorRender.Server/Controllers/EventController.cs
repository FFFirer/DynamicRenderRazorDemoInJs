using Microsoft.AspNetCore.Mvc;
using DynamicRazorRender.Shared;
using DynamicRazorRender.Server.Filters;

namespace DynamicRazorRender.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ApiResultWrapFilter]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        private readonly KeyEventBus<string> _eventBus;

        public EventController(ILogger<EventController> logger, KeyEventBus<string> eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        [HttpPost]
        public async Task RenderFile([FromBody] RenderFileRequest request)
        {
            await InternalRenderFromFile(request.FilePath);
        }

        [HttpPost]
        public async Task RenderPlain([FromBody] RenderPlainRequest request)
        {
            await InternalRenderFromPlain(request.Content);
        }

        [HttpPost]
        public async Task Render([FromBody] RenderRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RenderType))
            {
                return;
            }

            var result = request.RenderType switch
            {
                CustomEventConstants.RenderFromFile => InternalRenderFromFile(request.Content),
                CustomEventConstants.RenderFromPlain => InternalRenderFromPlain(request.Content),
                _ => Task.CompletedTask
            };

            await result;
        }

        protected async Task InternalRenderFromFile(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            if (!System.IO.File.Exists(filePath))
            {
                return;
            }

            await _eventBus.PushAsync(CustomEventConstants.RenderFromFile, filePath);
        }

        protected async Task InternalRenderFromPlain(string? plainText)
        {
            if (plainText == null)
            {
                plainText = string.Empty;
            }

            await _eventBus.PushAsync(CustomEventConstants.RenderFromPlain, plainText);
        }
    }
}