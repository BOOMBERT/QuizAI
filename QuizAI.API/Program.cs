using Microsoft.Extensions.FileProviders;
using QuizAI.API.Extensions;
using QuizAI.API.Middlewares;
using QuizAI.Application.Extensions;
using QuizAI.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

var publicStorageFolderName = "PublicUploads";
var privateStorageFolderName = "PrivateUploads";

var publicStoragePath = Path.Combine(builder.Environment.ContentRootPath, publicStorageFolderName);
var privateStoragePath = Path.Combine(builder.Environment.ContentRootPath, privateStorageFolderName);

Directory.CreateDirectory(publicStoragePath);
Directory.CreateDirectory(privateStoragePath);

builder.Services.AddInfrastructure(builder.Configuration);
builder.AddPresentation();
builder.Services.AddApplication(builder.Configuration, publicStoragePath, privateStoragePath);

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "PublicUploads")),
    RequestPath = "/api/uploads"
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
