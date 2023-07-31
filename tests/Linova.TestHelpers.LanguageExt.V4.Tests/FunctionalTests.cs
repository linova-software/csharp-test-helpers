using LanguageExt;
using LanguageExt.Common;

namespace Linova.TestHelpers.LanguageExt.V4.Tests;

[TestFixture]
public class FunctionalTests
{
    private static IEnumerable<Either<Error, bool>> ProvideEitherVariants()
    {
        yield return Shared.MakeExceptional<TestException>();
        yield return Shared.MakeExpected();
        yield return Shared.MakeRight(true);
    }

    [TestCaseSource(nameof(ProvideEitherVariants))]
    public void AssertThat_OnCreate_ExpectIsNotNull(Either<Error, bool> input)
    {
        var sot = input.AssertThat();
        Assert.That(sot, Is.Not.Null);
    }

    [Test]
    public void AssertThat_IsError_OnExceptional_ExpectPass()
    {
        _ = Shared.MakeExceptional<Exception>().AssertThat().IsError();
    }

    [Test]
    public void AssertThat_IsError_OnSpecificException_ExpectSpecificException()
    {
        var sot = Shared.MakeExceptional<ArgumentException>().AssertThat().IsError();

        Assert.That(sot.Exception.Case is ArgumentException, Is.True);
    }

    [Test]
    public void AssertThat_IsError_OnExpected_ExpectPass()
    {
        _ = Shared.MakeExpected().AssertThat().IsError();
    }

    [Test]
    public void AssertThat_IsError_OnManyErrorsZero_ExpectPass()
    {
        _ = Shared.MakeManyErrorsEmpty().AssertThat().IsError();
    }

    [Test]
    public void AssertThat_IsSuccess_OnRight_ExpectSuccess()
    {
        const bool expected = true;

        var sot = Shared.MakeRight(expected).AssertThat().IsSuccess();

        Assert.That(sot, Is.EqualTo(expected));
    }

    [Test]
    public void AssertThat_IsTestException_OnTestException_ExpectPass()
    {
        Shared.MakeExceptional<TestException>().AssertThat().IsTestException();
    }

    [Test]
    public void AssertThat_IsExpected_OnExpected_ExpectPass()
    {
        _ = Shared.MakeExpected().AssertThat().IsExpected();
    }

    [Test]
    public void AssertThat_IsExceptional_OnExceptional_ExpectPass()
    {
        _ = Shared.MakeExceptional<Exception>().AssertThat().IsExceptional();
    }

    [Test]
    public void AssertThat_AndIsExceptional_OnExceptional_ExpectPass()
    {
        _ = Shared.MakeExceptional<Exception>().AssertThat().IsError().AndIsExceptional();
    }

    [Test]
    public void AssertThat_AndIsExceptional_OnError_ExpectError()
    {
        var sot = Shared.MakeExceptional<TestException>();

        sot.AssertThat().IsError().AndIsExceptional();
    }

    [Test]
    public void AssertThat_AndHasUnderlyingException_OnSpecificException_ExpectPass()
    {
        Shared.MakeExceptional<ArgumentException>().AssertThat().IsExceptional()
            .AndHasUnderlyingException<ArgumentException>();
    }

    [Test]
    public void AssertThat_AndIsExpected_OnExpected_ExpectPass()
    {
        Shared.MakeExpected().AssertThat().IsError().AndIsExpected();
    }

    [Test]
    public void AssertThat_AndIsEmpty_OnExpected_ExpectPass()
    {
        Shared.MakeManyErrorsEmpty().AssertThat().IsError().AndIsEmpty();
    }

    [Test]
    public void AssertSome_OnSome_ExpectIdentity()
    {
        const int expected = 1;
        var sot = Shared.MakeOptionalSome(expected);

        sot.AssertSome(expected);
    }

    [Test]
    public void AssertSome_OnSomeReference_ExpectIdentity()
    {
        int? expected = 1;
        var sot = Shared.MakeOptionalSome(expected);

        sot.AssertSome(expected);
    }

    [Test]
    public void AssertNone_OnNone_ExpectPass()
    {
        Shared.MakeOptionalNone<int>().AssertNone();
    }

    [Test]
    public void AssertIsTrue_OnTrue_ExpectPass()
    {
        Shared.MakeOptionalSome(true).AssertIsTrue();
    }

    [Test]
    public void AssertIsFalse_OnFalse_ExpectPass()
    {
        Shared.MakeOptionalSome(false).AssertIsFalse();
    }
}