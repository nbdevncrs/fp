namespace TagsCloudCore.Infrastructure;

public struct Result<T>(string error, T value = default(T))
{
    public static implicit operator Result<T>(T v)
    {
        return ResultFactory.Ok(v);
    }

    public string Error { get; } = error;
    internal T Value { get; } = value;

    public bool IsSuccess => Error == null;

    public T GetValueOrThrow()
    {
        return IsSuccess ? Value : throw new InvalidOperationException($"No value. Only Error {Error}");
    }
}