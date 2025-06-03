using Microsoft.AspNetCore.Mvc;
using Nameplates.WebApp.Components;
using Scalar.AspNetCore;
using Syncfusion.Blazor;
using Syncfusion.Presentation;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(
    Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE")
);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddSyncfusionBlazor();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapGet("download.pptx", (
    [FromQuery(Name = "name")] string[] names,
    [FromQuery] double width = 7,
    [FromQuery] double height = 5.1,
    [FromQuery] bool inversed = false
) =>
{
    var widthInPts = width * 72;
    var heightInPts = height * 72;

    using var file = Presentation.Create() as Presentation ?? throw new InvalidOperationException();
    file.SlideSize.Type = SlideSizeType.Custom;
    file.SlideSize.Width = widthInPts;
    file.SlideSize.Height = heightInPts;

    foreach (var name in names)
    {
        var slide = file.Slides.Add(SlideLayoutType.Blank);
        var topTxt = slide.AddTextBox(0, 0, widthInPts, heightInPts / 2);
        topTxt.TextBody.VerticalAlignment = VerticalAlignmentType.Middle;
        var p = topTxt.TextBody.AddParagraph(name);
        p.Font.FontName = "Times New Roman";
        p.Font.FontSize = 44;
        p.HorizontalAlignment = HorizontalAlignmentType.Center;

        if (!inversed)
        {
            topTxt.Rotation = 180;
        }

        var bottomTxt = slide.AddTextBox(0, heightInPts / 2, widthInPts, heightInPts / 2);
        var pp = bottomTxt.TextBody.AddParagraph(name);
        pp.HorizontalAlignment = HorizontalAlignmentType.Center;
        pp.Font.FontName = "Times New Roman";
        pp.Font.FontSize = 44;
        bottomTxt.TextBody.VerticalAlignment = VerticalAlignmentType.Middle;

        if (inversed)
        {
            bottomTxt.Rotation = 180;
        }

        var divider = slide.Shapes.AddShape(AutoShapeType.Line, 0, heightInPts / 2, widthInPts, 0);
        divider.Fill.FillType = FillType.Solid;
        divider.Fill.SolidFill.Color = ColorObject.Black;
    }

    var stream = new MemoryStream();
    file.Save(stream);

    return Results.Stream(stream);
});

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapOpenApi();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Nameplates.WebApp.Client._Imports).Assembly);

app.MapScalarApiReference();

app.Run();