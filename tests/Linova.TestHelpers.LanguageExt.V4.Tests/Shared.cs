using LanguageExt;
using LanguageExt.Common;

namespace Linova.TestHelpers.LanguageExt.V4.Tests;

public static class Shared
{
   
    public static Either<Error, bool> MakeExceptional<TEx>() where TEx : Exception, new()
    {
        return Error.New(new TEx());
    }

    public static Either<Error, bool> MakeExpected()
    {
        return Error.New("expected error");
    }
    
    public static Either<Error, bool> MakeRight(bool val)
    {
        return val;
    }

    public static Either<Error, bool> MakeManyErrorsEmpty()
    {
        return new ManyErrors(new Seq<Error>());
    }

    public static Option<T> MakeOptionalSome<T>(T val)
    {
        return val;
    }
    
    public static Option<T> MakeOptionalNone<T>()
    {
        return Option<T>.None;
    }
}