using MS_FFMpeg.Services;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseHttpsRedirection();

app.MapGet("/api/test", () => "ASDasjdklasjdklajlks");

app.MapGet("/api/listItems", async () =>
{
    return await FunctionsHandler.ListItemsAsync();
});


app.MapGet("/api/data", () =>
{
    return Results.Ok(new { Message = "Hello, world!", Time = DateTime.Now });
});

app.MapGet("/api/audioInfo/{id}", async (string id) =>
{
    return Results.Ok(await FunctionsHandler.GetAudioInformation(id));
});

app.MapGet("/api/audioWaves/{id}", async (string id) =>
{
    return await FunctionsHandler.GetAudioWavePic(id);
});

app.MapGet("/api/applyEffects/{id}", async (string id, double speed, double volume, bool isNorm, string format) =>
{
    return await FunctionsHandler.ApplyEffects(id, speed, volume, isNorm, format);
});


app.Run();