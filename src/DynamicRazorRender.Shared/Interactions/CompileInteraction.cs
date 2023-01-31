using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace DynamicRazorRender.Shared
{
    public static class CompileInteraction
    {
        [JSInvokable]
        public static async Task CompileRazor(string code)
        {
            var keyEventBus = DynamicRenderRazorExtension.ServiceProvider?.GetRequiredService<KeyEventBus<string>>();

            await keyEventBus!.PushAsync(EventConstants.RenderComponent, code);
        }
    }
}