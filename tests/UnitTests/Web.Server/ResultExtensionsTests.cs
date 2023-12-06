using Application.Errors;
using Domain.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Web.Server.Extensions;

namespace UnitTests.Web.Server;

public class TestEntity { }

public class UnknownError : IError
{
    public List<IError> Reasons => throw new NotImplementedException();

    public string Message => throw new NotImplementedException();

    public Dictionary<string, object> Metadata => throw new NotImplementedException();
}

public class ResultExtensionsTests
{
    [Fact] 
    public void ToHttpResult_ShouldReturnOkObjectResultWithValue_WhenSuccessfulResultWithValue()
    {
        var result = Result.Ok(5).ToHttpResult();

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void ToHttpResult_ShouldReturnOkResult_WhenSuccessfulResultWithoutValue()
    {
        var result = Result.Ok().ToHttpResult();

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public void ToHttpResult_ShouldReturnStatusCode_WhenSuccessfulResultWithoutValueWithCustomStatusCode()
    {
        var statusCode = 111;
        var result = Result.Ok().ToHttpResult(statusCode);

        using(new AssertionScope())
        {
            result.Should().BeOfType<StatusCodeResult>();
            (result as StatusCodeResult)!.StatusCode.Should().Be(statusCode);
        }
        
    }

    [Fact]
    public void ToHttpResult_ShouldReturnNotFound_WhenErrorIsNotFound()
    {
        var id = Guid.NewGuid();
        var customResult = Result.Fail(new NotFoundError<TestEntity>(id));

        var result = customResult.ToHttpResult();

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void ToHttpResult_ShouldReturn500_WhenErrorIsApplicationError()
    {
        var result = Result.Fail(new ApplicationError("test")).ToHttpResult();

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void ToHttpResult_ShouldReturnBadRequest_WhenErrorIsDomainError()
    {
        var result = Result.Fail(new DomainError("test")).ToHttpResult();

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void ToHttpResult_ShouldReturnBadRequest_WhenErrorIsOfUnknownType()
    {
        var result = Result.Fail(new UnknownError()).ToHttpResult();

        using(new AssertionScope())
        {
            result.Should().BeOfType<StatusCodeResult>();
            (result as StatusCodeResult)!.StatusCode.Should().Be(500);
        }
    }
}
