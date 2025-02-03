using Microsoft.OpenApi.Models;
using QuizAI.API.Extensions;
using QuizAI.API.Middlewares;
using QuizAI.Application.Extensions;
using QuizAI.Domain.Entities;
using QuizAI.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddPresentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

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
