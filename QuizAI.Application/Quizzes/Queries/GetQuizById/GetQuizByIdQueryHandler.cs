using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Queries.GetQuizById;

public class GetQuizByIdQueryHandler : IRequestHandler<GetQuizByIdQuery, QuizDto>
{
    private readonly IMapper _mapper;
    private readonly IQuizAuthorizationService _quizAuthorizationService;
    private readonly IQuizzesRepository _quizzesRepository;

    public GetQuizByIdQueryHandler(IMapper mapper, IQuizAuthorizationService quizAuthorizationService, IQuizzesRepository quizzesRepository)
    {
        _mapper = mapper;
        _quizAuthorizationService = quizAuthorizationService;
        _quizzesRepository = quizzesRepository;
    }

    public async Task<QuizDto> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _quizzesRepository.GetAsync(request.QuizId, true, false, false) 
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        var canUserEditQuiz = await _quizAuthorizationService.AuthorizeReadOperationAndGetCanEditAsync(quiz, null);

        var quizDto = _mapper.Map<QuizDto>(quiz);
        return quizDto with { CanEdit = canUserEditQuiz };
    }
}
