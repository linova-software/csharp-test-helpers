using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using MediatR;
using Moq;
using NUnit.Framework;

namespace Linova.TestHelpers.MediatR;

public static class TestHelpers
{
    [DebuggerDisplay("Request = {Request}, Invocations = {Invocations}")]
    public sealed class InvocationTracker
    {
        public InvocationTracker(IBaseRequest request)
        {
            Request = request;
        }

        public IBaseRequest Request { get; }

        public int Invocations { get; private set; }

        public void OnCalled()
        {
            ++Invocations;
        }
    }

    public static void AssertInvocations(this InvocationTracker target, int expectedInvocations)
    {
        Assert.That(target.Invocations, Is.EqualTo(expectedInvocations));
    }

    public static InvocationTracker SetupRequest<TResponse>(
        this Mock<IMediator> mediator,
        Expression<Func<IRequest<TResponse>>> request,
        Func<TResponse> responder)
    {
        var requestFunc = request.Compile();
        var requestTracker = new InvocationTracker(requestFunc());
        mediator
            .Setup(MakeExpression<TResponse>(request))
            .ReturnsAsync(responder)
            .Callback(requestTracker.OnCalled);
        return requestTracker;
    }

    public static void SetupThrows<TResponse>(
        this Mock<IMediator> mediator,
        IRequest<TResponse> request)
    {
        SetupThrows(mediator, request, () => new TestException());
    }

    public static void SetupThrows<TResponse>(
        this Mock<IMediator> mediator,
        IRequest<TResponse> request,
        Func<Exception> responder)
    {
        mediator
            .Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
            .Throws(responder);
    }

    #region Details

    /// <summary>
    /// in order to support SetupRequest method taking Is.Any as parameter we have to construct the Send call ourselves
    /// </summary>
    private static Expression<Func<IMediator, Task<TResponse>>> MakeExpression<TResponse>(
        Expression<Func<IRequest<TResponse>>> request)
    {
        var requestExpression = Expression.Convert(request.Body, typeof(IRequest<TResponse>));
        var cancellationTokenExpr = Expression.Call(
            (Expression)null!,
            typeof(It).GetMethod("IsAny", BindingFlags.Public | BindingFlags.Static)!
                .MakeGenericMethod(typeof(CancellationToken)));
        var mediatrParam = Expression.Parameter(typeof(IMediator), "m");
        return Expression.Lambda<Func<IMediator, Task<TResponse>>>(
            Expression.Call(mediatrParam,
                typeof(ISender).GetMethods().First(m =>
                        m.Name == "Send" && m.ReturnType.GetGenericTypeDefinition() ==
                        typeof(Task<TResponse>).GetGenericTypeDefinition())
                    .MakeGenericMethod(
                        typeof(TResponse)), requestExpression, cancellationTokenExpr), mediatrParam);
    }

    #endregion
}