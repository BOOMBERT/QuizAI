using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace QuizAI.Application.Extensions.Tests;

public class GeneralValidationExtensionsTests
{
    private InlineValidator<DateTime?> GetIsNotFutureUtcDateTimeValidator()
    {
        var validator = new InlineValidator<DateTime?>();
        validator.RuleFor(x => x).IsNotFutureUtcDateTime();

        return validator;
    }

    [Theory]
    [InlineData(0)] // now
    [InlineData(-1)] // 1 minute
    [InlineData(-60)] // 1 hour
    [InlineData(-1440)] // 1 day
    [InlineData(-5256000)] // ~ 10 years
    public void IsNotFutureUtcDateTime_WhenValidUtcDateTime_ShouldNotHaveValidationErrors(int minutesOffset)
    {
        // Arrange

        var validUtcDateTime = DateTime.UtcNow.AddMinutes(minutesOffset);

        // Act

        var result = GetIsNotFutureUtcDateTimeValidator().TestValidate(validUtcDateTime);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(1)] // 1 minute
    [InlineData(60)] // 1 hour
    [InlineData(1440)] // 1 day
    [InlineData(5256000)] // ~ 10 years
    public void IsNotFutureUtcDateTime_WhenFutureUtcDateTime_ShouldHaveValidationError(int minutesOffset)
    {
        // Arrange

        var futureUtcDateTime = DateTime.UtcNow.AddMinutes(minutesOffset);

        // Act

        var result = GetIsNotFutureUtcDateTimeValidator().TestValidate(futureUtcDateTime);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("The date cannot exceed the current date");
    }
}
