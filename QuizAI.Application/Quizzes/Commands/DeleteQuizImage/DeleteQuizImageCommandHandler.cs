using AutoMapper;
using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.DeleteQuizImage;

public class DeleteQuizImageCommandHandler : IRequestHandler<DeleteQuizImageCommand, NewQuizId>
{
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;

    public DeleteQuizImageCommandHandler(IRepository repository, IQuizService quizService)
    {
        _repository = repository;
        _quizService = quizService;
    }

    public async Task<NewQuizId> Handle(DeleteQuizImageCommand request, CancellationToken cancellationToken)
    {
        var newQuiz = await _quizService.GetNewAndDeprecateOldAsync(request.GetId());

        if (await _repository.GetFieldAsync<Quiz, Guid?>(request.GetId(), "ImageId") == null)
            throw new NotFoundException($"Quiz with ID {request.GetId()} has no associated image.");

        newQuiz.ImageId = null;

        await _repository.AddAsync(newQuiz);
        await _repository.SaveChangesAsync();

        return new NewQuizId(newQuiz.Id);
    }
}
