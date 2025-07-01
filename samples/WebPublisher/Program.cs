using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();

app.MapPost("/publish", (string sourceDir, string targetDir) =>
{
    if (!Directory.Exists(sourceDir))
    {
        return Results.BadRequest($"Source directory '{sourceDir}' does not exist.");
    }

    if (!Directory.Exists(targetDir))
    {
        Directory.CreateDirectory(targetDir);
    }

    foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
    {
        var relativePath = Path.GetRelativePath(sourceDir, file);
        var targetPath = Path.Combine(targetDir, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
        System.IO.File.Copy(file, targetPath, true);
    }

    return Results.Ok($"Files copied from {sourceDir} to {targetDir}");
});

app.Run();
