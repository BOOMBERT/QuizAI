using AutoMapper;
using MediatR;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Queries.GetQuizById;

public class GetQuizByIdQueryHandler : IRequestHandler<GetQuizByIdQuery, QuizDto>
{
    private readonly IMapper _mapper;
    private readonly IQuizzesRepository _quizzesRepository;

    public GetQuizByIdQueryHandler(IMapper mapper, IQuizzesRepository quizzesRepository)
    {
        _mapper = mapper;
        _quizzesRepository = quizzesRepository;
    }

    public async Task<QuizDto> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _quizzesRepository.GetWithCategoriesAsync(request.QuizId) 
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        return _mapper.Map<QuizDto>(quiz);
    }
}
