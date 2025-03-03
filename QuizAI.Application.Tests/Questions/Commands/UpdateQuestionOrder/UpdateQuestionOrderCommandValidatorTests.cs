using FluentValidation.TestHelper;
using QuizAI.Application.Questions.Dtos;
using Xunit;

namespace QuizAI.Application.Questions.Commands.UpdateQuestionOrder.Tests;

public class UpdateQuestionOrderCommandValidatorTests
{
    private (UpdateQuestionOrderCommand, UpdateQuestionOrderCommandValidator) GetUpdateQuestionOrderCommandAndValidator(ICollection<UpdateQuestionOrderDto> orderChanges)
        => (new UpdateQuestionOrderCommand() { OrderChanges = orderChanges}, new UpdateQuestionOrderCommandValidator());

    public static IEnumerable<object[]> validOrderChanges => 
        new List<object[]>
        {
            new object[] { new List<UpdateQuestionOrderDto> { new(1, 2), new(2, 1) } },
            new object[] { new List<UpdateQuestionOrderDto> { new(2, 3), new(1, 1), new(3, 2) } }
        };

    [Theory]
    [MemberData(nameof(validOrderChanges))]
    public void Validator_WhenValidCommand_ShouldNotHaveValidationErrors(ICollection<UpdateQuestionOrderDto> validOrderChanges)
    {
        // Arrange

        var (command, validator) = GetUpdateQuestionOrderCommandAndValidator(validOrderChanges);

        // Act

        var result = validator.TestValidate(command);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_WhenEmptyOrderChanges_ShouldHaveValidationError()
    {
        // Arrange

        var (command, validator) = GetUpdateQuestionOrderCommandAndValidator(new List<UpdateQuestionOrderDto>());

        // Act

        var result = validator.TestValidate(command);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.OrderChanges)
            .WithErrorMessage("Order changes cannot be empty");
    }

    public static IEnumerable<object[]> orderChangesWithoutContinuousSequence => 
        new List<object[]>
        {
            new object[] { new List<UpdateQuestionOrderDto> { new(1, 2) } },
            new object[] { new List<UpdateQuestionOrderDto> { new(2, 1), new(1, 3) } },
            new object[] { new List<UpdateQuestionOrderDto> { new(2, 1), new(1, 1) } }
        };

    [Theory]
    [MemberData(nameof(orderChangesWithoutContinuousSequence))]
    public void Validator_WhenNoContinuousSequenceOfOrderChanges_ShouldHaveValidationError(ICollection<UpdateQuestionOrderDto> orderChanges)
    {
        // Arrange

        var (command, validator) = GetUpdateQuestionOrderCommandAndValidator(orderChanges);

        // Act

        var result = validator.TestValidate(command);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.OrderChanges)
            .WithErrorMessage("New orders must form a continuous sequence without gaps or duplicates");
    }

    public static IEnumerable<object[]> orderChangesWithoutUniqueQuestionIds =>
        new List<object[]>
        {
                new object[] { new List<UpdateQuestionOrderDto> { new(1, 1), new(1, 2) } },
                new object[] { new List<UpdateQuestionOrderDto> { new(1, 2), new(2, 1), new(1, 3) } }
        };

    [Theory]
    [MemberData(nameof(orderChangesWithoutUniqueQuestionIds))]
    public void Validator_WhenNoUniqueQuestionIdsInOrderChanges_ShouldHaveValidationError(ICollection<UpdateQuestionOrderDto> orderChanges)
    {
        // Arrange

        var (command, validator) = GetUpdateQuestionOrderCommandAndValidator(orderChanges);

        // Act

        var result = validator.TestValidate(command);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.OrderChanges)
            .WithErrorMessage("Question IDs must be unique");
    }
}
