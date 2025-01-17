using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Services;

public class QuizService : IQuizService
{
    private readonly IMapper _mapper;
    private readonly IQuizzesRepository _quizzesRepository;

    public QuizService(IMapper mapper, IQuizzesRepository quizzesRepository)
    {
        _mapper = mapper;
        _quizzesRepository = quizzesRepository;
    }

    public async Task<Quiz> GetNewAndDeprecateOldAsync(Guid oldQuizId)
    {
        var oldQuiz = await _quizzesRepository.GetAsync(oldQuizId, true, true)
            ?? throw new NotFoundException($"Quiz with ID {oldQuizId} was not found");

        if (oldQuiz.IsDeprecated)
            throw new NotFoundException($"Quiz with ID {oldQuizId} was not found");

        var newQuiz = _mapper.Map<Quiz>(oldQuiz);

        await DeprecateAsync(oldQuiz, newQuiz.Id);

        return newQuiz;
    }

    private async Task DeprecateAsync(Quiz oldQuiz, Guid newQuizId)
    {
        await _quizzesRepository.UpdateLatestVersionIdAsync(oldQuiz.Id, newQuizId);
        
        oldQuiz.LatestVersionId = newQuizId;
        oldQuiz.Categories.Clear();
        oldQuiz.IsDeprecated = true;
    }
}
