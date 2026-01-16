namespace TagsCloudCore.Infrastructure;

public static class ResultExtensions
{
    public static Result<TOutput> Then<TInput, TOutput>(
        this Result<TInput> input,
        Func<TInput, TOutput> continuation)
    {
        return input.Then(inp => ResultFactory.Of(() => continuation(inp)));
    }

    public static Result<None> Then<TInput, TOutput>(
        this Result<TInput> input,
        Action<TInput> continuation)
    {
        return input.Then(inp => ResultFactory.OfAction(() => continuation(inp)));
    }

    public static Result<None> Then<TInput>(
        this Result<TInput> input,
        Action<TInput> continuation)
    {
        return input.Then(inp => ResultFactory.OfAction(() => continuation(inp)));
    }

    public static Result<TOutput> Then<TInput, TOutput>(
        this Result<TInput> input,
        Func<TInput, Result<TOutput>> continuation)
    {
        return input.IsSuccess
            ? continuation(input.Value)
            : ResultFactory.Fail<TOutput>(input.Error);
    }

    public static Result<TInput> OnFail<TInput>(
        this Result<TInput> input,
        Action<string> handleError)
    {
        if (!input.IsSuccess)
            handleError(input.Error);

        return input;
    }

    public static Result<TInput> ReplaceError<TInput>(
        this Result<TInput> input,
        Func<string, string> replaceError)
    {
        return input.IsSuccess ? input : ResultFactory.Fail<TInput>(replaceError(input.Error));
    }

    public static Result<TInput> RefineError<TInput>(
        this Result<TInput> input,
        string errorMessage)
    {
        return input.ReplaceError(err => errorMessage + ". " + err);
    }
}