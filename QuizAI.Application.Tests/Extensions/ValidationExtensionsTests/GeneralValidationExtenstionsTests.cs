using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace QuizAI.Application.Extensions.Tests;

public class GeneralValidationExtensionsTests
{
    [Theory]
    [InlineData(0)] // now
    [InlineData(-1)] // 1 minute
    [InlineData(-60)] // 1 hour
    [InlineData(-1440)] // 1 day
    [InlineData(-5256000)] // ~ 10 years
    public void IsNotFutureUtcDateTime_WhenValidUtcDateTime_ShouldNotHaveValidationErrors(int minutesOffset)
    {
        // arrange
        
        var validator = new InlineValidator<DateTime?>();
        validator.RuleFor(x => x).IsNotFutureUtcDateTime();

        var validUtcDateTime = DateTime.UtcNow.AddMinutes(minutesOffset);

        // act

        var result = validator.TestValidate(validUtcDateTime);

        // assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(1)] // 1 minute
    [InlineData(60)] // 1 hour
    [InlineData(1440)] // 1 day
    [InlineData(5256000)] // ~ 10 years
    public void IsNotFutureUtcDateTime_WhenFutureUtcDateTime_ShouldHaveValidationError(int minutesOffset)
    {
        // arrange

        var validator = new InlineValidator<DateTime?>();
        validator.RuleFor(x => x).IsNotFutureUtcDateTime();

        var futureUtcDateTime = DateTime.UtcNow.AddMinutes(minutesOffset);

        // act

        var result = validator.TestValidate(futureUtcDateTime);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("The date cannot exceed the current date");
    }
}
