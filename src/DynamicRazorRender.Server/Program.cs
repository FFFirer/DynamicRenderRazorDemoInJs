using DynamicRazorRender.Server.Events;
using DynamicRazorRender.Server.Filters;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => {
    options.Filters.Add<HttpResponseExceptionFilter>();
});

builder.Logging.ClearProviders().AddConsole();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddDynamicRazorRenderShared(codeEnvironment: DynamicRazorRender.Shared.CodeEnvironment.Server);
builder.Services.AddSingleton(typeof(EventBus<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.Services.UseServiceProvider();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
