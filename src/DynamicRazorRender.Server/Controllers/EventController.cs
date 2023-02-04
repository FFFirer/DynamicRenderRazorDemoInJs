using Microsoft.AspNetCore.Mvc;
using DynamicRazorRender.Shared;
using DynamicRazorRender.Server.Filters;
using DynamicRazorRender.Server.Events;

namespace DynamicRazorRender.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ApiResultWrapFilter]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        private readonly KeyEventBus<string> _eventBus;
        private readonly EventBus<string> _eventCenter;

        public EventController(ILogger<EventController> logger, KeyEventBus<string> eventBus, EventBus<string> eventCenter)
        {
            _logger = logger;
            _eventBus = eventBus;
            _eventCenter = eventCenter;
        }

        [HttpPost]
        public async Task RenderFile([FromBody] RenderFileRequest request)
        {
            await InternalRenderFromFile(request.FilePath);
        }

        [HttpPost]
        public async Task RenderPlain([FromBody] RenderPlainRequest request)
        {
            await InternalRenderFromPlain(request.PlainText);
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

        protected Task InternalRenderFromFile(string? filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath)
                && System.IO.File.Exists(filePath))
            {
                _eventCenter.Emit(CustomEventConstants.RenderFromFile, filePath);
            }

            return Task.CompletedTask;
        }

        protected Task InternalRenderFromPlain(string? plainText)
        {
            if (plainText == null)
            {
                plainText = string.Empty;
            }

            _eventCenter.EmitAsync(CustomEventConstants.RenderFromPlain, plainText);

            return Task.CompletedTask;
        }
    }
}