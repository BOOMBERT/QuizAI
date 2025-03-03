namespace QuizAI.Application.Tests.TestHelpers;

internal static class AnswerTestHelper
{
    internal static List<string> GenerateAnswersExceedingMaxTotalLength(int numberOfAnswers, int maxTotalLength)
    {
        int lengthPerAnswer = (int)Math.Ceiling((double)(maxTotalLength + 1) / numberOfAnswers);
        return Enumerable.Repeat(new string('A', lengthPerAnswer), numberOfAnswers).ToList();
    }
}
