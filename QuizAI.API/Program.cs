using Microsoft.Extensions.FileProviders;
using QuizAI.API.Extensions;
using QuizAI.API.Middlewares;
using QuizAI.Application.Extensions;
using QuizAI.Domain.Entities;
using QuizAI.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

var publicStorageFolderName = "PublicUploads";
var privateStorageFolderName = "PrivateUploads";

var publicStoragePath = Path.Combine(builder.Environment.ContentRootPath, publicStorageFolderName);
var privateStoragePath = Path.Combine(builder.Environment.ContentRootPath, privateStorageFolderName);

Directory.CreateDirectory(publicStoragePath);
Directory.CreateDirectory(privateStoragePath);

builder.AddPresentation();
builder.Services.AddApplication(builder.Configuration, publicStoragePath, privateStoragePath);
builder.Services.AddInfrastructure(builder.Configuration);

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

app.MapGroup("api/identity").MapIdentityApi<User>();

app.UseAuthorization();

app.MapControllers();

app.Run();
