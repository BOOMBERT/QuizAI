﻿using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Application.Users;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;
        
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
        services.AddAutoMapper(applicationAssembly);
        services.AddValidatorsFromAssembly(applicationAssembly)
            .AddFluentValidationAutoValidation();

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        var storageFolderName = "Uploads";

        services.AddScoped<IFileStorageService, FileStorageService>(provider =>
        {
            var environment = provider.GetRequiredService<IHostEnvironment>();
            return new FileStorageService(environment, storageFolderName);
        });

        services.AddScoped<IImageService, ImageService>(provider =>
        {
            var repository = provider.GetRequiredService<IRepository>();
            var fileStorageService = provider.GetRequiredService<IFileStorageService>();
            var imagesRepository = provider.GetRequiredService<IImagesRepository>();
            var questionsRepository = provider.GetRequiredService<IQuestionsRepository>();
            (ushort, ushort) imagesDefaultSize = (800, 800);
            return new ImageService(repository, fileStorageService, imagesRepository, questionsRepository, imagesDefaultSize);
        });

        services.AddScoped<ICategoryService, CategoryService>();

        services.AddScoped<IQuestionService, QuestionService>(provider =>
        {
            var mapper = provider.GetRequiredService<IMapper>();
            var repository = provider.GetRequiredService<IRepository>();
            byte maxNumberOfQuestions = 20;
            return new QuestionService(mapper, repository, maxNumberOfQuestions);
        });

        services.AddScoped<IQuizService, QuizService>();
    }
}
