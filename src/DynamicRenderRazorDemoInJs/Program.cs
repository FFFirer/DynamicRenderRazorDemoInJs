using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DynamicRenderRazorDemoInJs;
using Masa.Blazor.Extensions.Languages.Razor;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Razor.Language;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var app = builder.Build();

RazorCompile.Initialized(await GetReferences(app.Services), GetRazorExtension());

await app.RunAsync();

async Task<List<PortableExecutableReference>?> GetReferences(IServiceProvider services)
{
	#region WebAssembly

	var httpClient = services.GetRequiredService<HttpClient>();

	var portableExecutableReferences = new List<PortableExecutableReference>();

	foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
	{
		try
		{
			var stream = await httpClient.GetStreamAsync($"_framework/{assembly.GetName().Name}.dll");

			if(stream.Length > 0)
			{
				portableExecutableReferences.Add(MetadataReference.CreateFromStream(stream));
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}

	return portableExecutableReferences;
	#endregion
}

List<RazorExtension> GetRazorExtension()
{
    var razorExtension = new List<RazorExtension>();

    foreach (var asm in typeof(Program).Assembly.GetReferencedAssemblies())
    {
        razorExtension.Add(new AssemblyExtension(asm.FullName, AppDomain.CurrentDomain.Load(asm.FullName)));
    }

    return razorExtension;
}
