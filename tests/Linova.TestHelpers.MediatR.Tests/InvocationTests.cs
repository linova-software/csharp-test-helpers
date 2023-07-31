using MediatR;
using Moq;

namespace Linova.TestHelpers.MediatR.Tests;

public class Tests
{
    public record TestRequest(string Value) : IRequest<bool>;

    public record TestRequestNoValue() : IRequest<bool>;

    [Test]
    public void InvocationTracker_OnConstructor_ExpectDefault()
    {
        var sot = new TestHelpers.InvocationTracker(null!);
        sot.AssertInvocations(0);
    }

    [Test]
    public async Task InvocationTracker_OnSendTestRequest_ExpectTrue()
    {
        var sot = new Mock<IMediator>(MockBehavior.Strict);
        var request = new TestRequest(Guid.NewGuid().ToString());
        var invocationTracker = sot.SetupRequest(
            () => request,
            () => true);

        var result = await sot.Object.Send(request);

        Assert.That(result, Is.True);
        Assert.That(invocationTracker.Invocations, Is.EqualTo(1));
    }

    [Test]
    public async Task InvocationTracker_OnSendAnyTestRequest_ExpectTrue()
    {
        var sot = new Mock<IMediator>(MockBehavior.Strict);
        var invocationTracker = sot.SetupRequest(
            () => It.IsAny<TestRequest>(),
            () => true);

        var result = await sot.Object.Send(It.IsAny<TestRequest>());

        Assert.That(result, Is.True);
        invocationTracker.AssertInvocations(1);
    }

    [Test]
    public async Task InvocationTracker_OnSendAnyTestRequest_ExpectSpecificRequestTrue()
    {
        var sot = new Mock<IMediator>(MockBehavior.Strict);
        var invocationTracker = sot.SetupRequest(
            () => It.IsAny<TestRequest>(),
            () => true);

        var result = await sot.Object.Send(new TestRequest(string.Empty));

        Assert.That(result, Is.True);
        invocationTracker.AssertInvocations(1);
    }

    [Test]
    public async Task InvocationTracker_OnSendAnyRequestWithInterface_ExpectTrue()
    {
        var sot = new Mock<IMediator>(MockBehavior.Strict);
        var invocationTracker = sot.SetupRequest(
            () => It.IsAny<IRequest<bool>>(),
            () => true);

        var result = await sot.Object.Send(It.IsAny<IRequest<bool>>());

        Assert.That(result, Is.True);
        invocationTracker.AssertInvocations(1);
    }

    [Test]
    public async Task InvocationTracker_OnSendAnyRequestWithInterface_ExpectSpecificTrue()
    {
        var sot = new Mock<IMediator>(MockBehavior.Strict);
        var invocationTracker = sot.SetupRequest(
            () => It.IsAny<IRequest<bool>>(),
            () => true);

        var result = await sot.Object.Send(new TestRequestNoValue());

        Assert.That(result, Is.True);
        invocationTracker.AssertInvocations(1);
    }

    [Test]
    public void InvocationTracker_OnDifferentRequest_ExpectThrows()
    {
        var sot = new Mock<IMediator>(MockBehavior.Strict);
        _ = sot.SetupRequest(
            () => It.IsAny<TestRequest>(),
            () => true);

        Assert.ThrowsAsync<MockException>(async () => { await sot.Object.Send(new TestRequestNoValue()); });
    }

    [Test]
    public void InvocationTracker_OnSetupThrowsSpecific_ExpectThrows()
    {
        var sot = new Mock<IMediator>(MockBehavior.Strict);
        var request = new TestRequest(Guid.NewGuid().ToString());
        sot.SetupThrows(request);

        Assert.ThrowsAsync<MockException>(async () => { await sot.Object.Send(new TestRequestNoValue()); });
    }

    [Test]
    public void InvocationTracker_OnSetupThrowsAny_ExpectThrows()
    {
        var sot = new Mock<IMediator>(MockBehavior.Strict);
        sot.SetupThrows(It.IsAny<TestRequest>());

        Assert.ThrowsAsync<MockException>(async () => { await sot.Object.Send(new TestRequestNoValue()); });
    }
}