namespace ParserLib
{
    using System;
    public static class ResultExtensions
    {
        public static IResult<U> IfSuccess<T, U>(this IResult<T> lhs, Func<IResult<T>, IResult<U>> next)
        {
            if (lhs == null)
            {
                throw new ArgumentNullException(nameof(lhs));
            }

            if (lhs.WasSuccessful)
            {
                return next(lhs);
            }

            return Result.Fail<U>(lhs.Remainder,
                lhs.Message, lhs.Expectations);
        }

        public static IResult<T> IfFailure<T>(this IResult<T> lhs, Func<IResult<T>, IResult<T>> next)
        {
            if (lhs == null)
            {
                throw new ArgumentNullException(nameof(lhs));
            }

            if (lhs.WasSuccessful)
            {
                return lhs;
            }

            return next(lhs);
        }
    }
}