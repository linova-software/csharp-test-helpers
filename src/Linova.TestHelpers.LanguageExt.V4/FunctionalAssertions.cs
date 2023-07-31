using LanguageExt;
using LanguageExt.Common;
using LanguageExt.UnsafeValueAccess;
using NUnit.Framework;

namespace Linova.TestHelpers.LanguageExt.V4;

public static class FunctionalAssertions
{
    public static IEitherAssert<T> AssertThat<T>(this Either<Error, T> result)
    {
        return new EitherAssert<T>(result);
    }

    public static void AssertSome<T>(this Option<T> self, T some)
    {
        Assert.IsTrue(self.IsSome, "Option is None, but expected Some(...)");
        Assert.AreEqual(some, self.ValueUnsafe());
    }

    public static void AssertNone<T>(this Option<T> self)
    {
        Assert.IsTrue(self.IsNone, "Option is Some(...), but expected None");
    }

    public static void AssertIsTrue(this Option<bool> self)
    {
        Assert.IsTrue(self.IsSome, "Option is None, but expected Some(true)");
        Assert.IsTrue(self.ValueUnsafe());
    }

    public static void AssertIsFalse(this Option<bool> self)
    {
        Assert.IsTrue(self.IsSome, "Option is None, but expected Some(false)");
        Assert.IsFalse(self.ValueUnsafe());
    }

    public static T AndIsExceptional<T>(this T self) where T : Error
    {
        Assert.IsTrue(self.IsExceptional);

        return self;
    }

    public static void AndHasUnderlyingException<TException>(this Error self) where TException : Exception
    {
        Assert.IsTrue(self.Is<TException>());
    }

    public static T AndIsExpected<T>(this T self) where T : Error
    {
        Assert.IsTrue(self.IsExpected);

        return self;
    }

    public static T AndIsEmpty<T>(this T self) where T : Error
    {
        Assert.IsTrue(self.IsEmpty);

        return self;
    }

    public static Error IsError<T>(this IEitherAssert<T> self)
    {
        return self.IsError<Error>();
    }

    public static Expected IsExpected<T>(this IEitherAssert<T> self)
    {
        return self.IsError<Expected>();
    }

    public static Exceptional IsExceptional<T>(this IEitherAssert<T> self)
    {
        return self.IsError<Exceptional>();
    }

    public static void IsTestException<T>(this IEitherAssert<T> self)
    {
        self
            .IsError()
            .AndIsExceptional()
            .AndHasUnderlyingException<TestException>();
    }

    private class EitherAssert<T> : IEitherAssert<T>
    {
        private readonly Either<Error, T> _result;

        public EitherAssert(Either<Error, T> result)
        {
            _result = result;
        }

        public TError IsError<TError>() where TError : Error
        {
            Assert.IsTrue(_result.IsLeft, "Should be error");

            var resultCase = _result.Case;
            Assert.IsInstanceOf<TError>(resultCase, "Should be error type");

            return (TError)resultCase;
        }

        public T IsSuccess()
        {
            _result.IfLeft(e => e.Throw());

            var resultCase = _result.Case;
            Assert.IsInstanceOf<T>(resultCase, "Should be right");

            return (T)resultCase;
        }
    }
}

public interface IEitherAssert<out T>
{
    TError IsError<TError>() where TError : Error;

    T IsSuccess();
}