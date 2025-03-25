using QuizAI.API.Extensions;
using QuizAI.API.Middlewares;
using QuizAI.Application.Extensions;
using QuizAI.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure();
builder.AddPresentation();
builder.Services.AddApplication(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") != "Testing")
{
    app.ApplyMigrations();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
