using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Users;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.QuizPermissions.Commands.ManageUsersQuizPermissions;

public class ManageUsersQuizPermissionsCommandHandler : IRequestHandler<ManageUsersQuizPermissionsCommand>
{
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;
    private readonly IUserRepository _userRepository;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuizPermissionsRepository _quizPermissionsRepository;

    public ManageUsersQuizPermissionsCommandHandler(
        IRepository repository, IUserContext userContext, IUserRepository userRepository, IQuizzesRepository quizzesRepository, IQuizPermissionsRepository quizPermissionsRepository)
    {
        _repository = repository;
        _userContext = userContext;
        _userRepository = userRepository;
        _quizzesRepository = quizzesRepository;
        _quizPermissionsRepository = quizPermissionsRepository;
    }

    public async Task Handle(ManageUsersQuizPermissionsCommand request, CancellationToken cancellationToken)
    {
        var (quizId, userEmail) = (request.GetQuizId(), request.GetUserEmail());
        var currentUser = _userContext.GetCurrentUser();

        if (string.Equals(userEmail, currentUser.Email, StringComparison.OrdinalIgnoreCase)) 
            throw new ConflictException("You cannot assign permissions to yourself");

        var userId = await _userRepository.GetIdAsync(userEmail)
            ?? throw new NotFoundException($"User with email {userEmail} was not found");

        var (creatorId, isPrivate, isDeprecated) = await _quizzesRepository.GetCreatorIdAndIsPrivateAndIsDeprecatedAsync(quizId)
            ?? throw new NotFoundException($"Quiz with ID {quizId} was not found");

        if (isDeprecated) 
            throw new NotFoundException($"Quiz with ID {quizId} was not found");

        if (creatorId != currentUser.Id) 
            throw new ConflictException($"You cannot manage permissions for quiz with ID {quizId} because you are not its creator");

        var userQuizPermissions = await _quizPermissionsRepository.GetAsync(quizId, userId);
        var userQuizPermissionsExist = userQuizPermissions != null;

        if (!request.CanEdit && !request.CanPlay)
        {
            if (!userQuizPermissionsExist)
                throw new ConflictException(
                    $"Cannot remove permissions for user {userEmail} " +
                    $"because the user already does not have any additional permissions for quiz with ID {quizId}");

            _repository.Remove(userQuizPermissions!);
            await _repository.SaveChangesAsync();
            return;
        }

        if (userQuizPermissionsExist && !request.CanEdit && request.CanPlay && !isPrivate)
        {
            _repository.Remove(userQuizPermissions!);
            await _repository.SaveChangesAsync();
            return;
        }

        if (!request.CanEdit && request.CanPlay && !isPrivate) 
            throw new ConflictException($"User already can play quiz with Id {quizId} because it is public");

        if (!request.CanPlay && !isPrivate) 
            throw new ConflictException($"You cannot take away permission to play from quiz with ID {quizId} because it is public");

        if (!userQuizPermissionsExist)
        {
            var newUserQuizPermissions = new QuizPermission { 
                QuizId = quizId, 
                UserId = userId, 
                CanEdit = request.CanEdit, 
                CanPlay = request.CanPlay 
            };

            await _repository.AddAsync(newUserQuizPermissions);
        }
        else
        {
            if (userQuizPermissions!.CanEdit != request.CanEdit) userQuizPermissions.CanEdit = request.CanEdit;
            if (userQuizPermissions.CanPlay != request.CanPlay) userQuizPermissions.CanPlay = request.CanPlay;
        }

        await _repository.SaveChangesAsync();
    }
}
