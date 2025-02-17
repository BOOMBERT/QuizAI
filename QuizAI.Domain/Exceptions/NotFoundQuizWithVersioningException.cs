namespace QuizAI.Domain.Exceptions;

public class NotFoundQuizWithVersioningException : NotFoundException
{
    public NotFoundQuizWithVersioningException(Guid quizId, Guid? latestVersionId)
        : base(BuildMessage(quizId, latestVersionId)) { }

    private static string BuildMessage(Guid quizId, Guid? latestVersionId)
    {
        return $"The quiz with ID {quizId} was not found - " +
               (latestVersionId == null ?
                "No latest version is available" :
                $"The latest version ID is {latestVersionId}");
    }
}
