namespace ParserLib
{
    public delegate IResult<T> Parser<out T>(IInput input);
}