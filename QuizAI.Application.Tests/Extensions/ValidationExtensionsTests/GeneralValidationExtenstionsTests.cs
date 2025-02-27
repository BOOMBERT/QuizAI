using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace QuizAI.Application.Extensions.Tests;

public class GeneralValidationExtensionsTests
{
    private readonly InlineValidator<DateTime?> _IsNotFutureUtcDateTimeValidator;

    public GeneralValidationExtensionsTests()
    {
        _IsNotFutureUtcDateTimeValidator = new InlineValidator<DateTime?>();
        _IsNotFutureUtcDateTimeValidator.RuleFor(x => x).IsNotFutureUtcDateTime();
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

        var result = _IsNotFutureUtcDateTimeValidator.TestValidate(validUtcDateTime);

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

        var result = _IsNotFutureUtcDateTimeValidator.TestValidate(futureUtcDateTime);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("The date cannot exceed the current date");
    }
}
