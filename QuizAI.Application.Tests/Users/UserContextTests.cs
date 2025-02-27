using Xunit;
using Microsoft.AspNetCore.Http;
using QuizAI.Domain.Exceptions;
using System.Security.Claims;
using Moq;

namespace QuizAI.Application.Users.Tests;

public class UserContextTests
{
    [Theory]
    [InlineData("1", "test@test.com")]
    [InlineData("2", "user@example.com")]
    public void GetCurrentUser_WhenAuthenticatedUser_ShouldReturnCurrentUser(string userId, string email)
    {
        // Arrange

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email)
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthType"));

        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()
        {
            User = user
        });

        var userContext = new UserContext(httpContextAccessorMock.Object);

        // Act

        var currentUser = userContext.GetCurrentUser();

        // Assert

        Assert.Equal(userId, currentUser.Id);
        Assert.Equal(email, currentUser.Email);
    }

    [Fact]
    public void GetCurrentUser_WhenUnauthenticatedUser_ShouldThrowUnauthorizedException()
    {
        // Arrange

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, "1"),
            new(ClaimTypes.Email, "test@test.com")
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()
        {
            User = user
        });

        var userContext = new UserContext(httpContextAccessorMock.Object);

        // Act

        Action action = () => userContext.GetCurrentUser();

        // Assert

        var exception = Assert.Throws<UnauthorizedException>(action);
        Assert.Equal("User is not authenticated", exception.Message);
    }

    [Fact]
    public void GetCurrentUser_WhenUserIdentityIsNull_ShouldThrowUnauthorizedException()
    {
        // Arrange

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal();

        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()
        {
            User = user
        });

        var userContext = new UserContext(httpContextAccessorMock.Object);

        // Act

        Action action = () => userContext.GetCurrentUser();

        // Assert

        var exception = Assert.Throws<UnauthorizedException>(action);
        Assert.Equal("User is not authenticated", exception.Message);
    }

    [Fact]
    public void GetCurrentUser_WhenUserContextNotPresent_ShouldThrowBadRequestException()
    {
        // Arrange

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null);

        var userContext = new UserContext(httpContextAccessorMock.Object);

        // Act

        Action action = () => userContext.GetCurrentUser();

        // Assert

        var exception = Assert.Throws<BadRequestException>(action);
        Assert.Equal("User context is not present", exception.Message);
    }
}
