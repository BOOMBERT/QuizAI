﻿using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.DeleteQuiz;

public class DeleteQuizCommandHandler : IRequestHandler<DeleteQuizCommand>
{
    private readonly IRepository _repository;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly ICategoryService _categoryService;
    private readonly IImageService _imageService;

    public DeleteQuizCommandHandler(IRepository repository, IQuizzesRepository quizzesRepository, ICategoryService categoryService, IImageService imageService)
    {
        _repository = repository;
        _quizzesRepository = quizzesRepository;
        _categoryService = categoryService;
        _imageService = imageService;
    }

    public async Task Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _quizzesRepository.GetAsync(request.GetId(), true);
        
        if (quiz == null || quiz.IsDeprecated)
            throw new NotFoundException($"Quiz with ID {request.GetId()} was not found");

        await _categoryService.RemoveUnusedAsync(quiz, Enumerable.Empty<string>());

        if (!await _quizzesRepository.HasAnyAttemptsAsync(quiz.Id))
        {
            _repository.Remove(quiz);
            await _quizzesRepository.UpdateLatestVersionIdAsync(quiz.Id, null);

            if (quiz.ImageId != null)
                await _imageService.DeleteIfNotAssignedAsync((Guid)quiz.ImageId, quiz.Id);
        }
        else
        {
            quiz.IsDeprecated = true;
        }

        await _repository.SaveChangesAsync();
    }
}
