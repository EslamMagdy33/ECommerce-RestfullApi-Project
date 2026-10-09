namespace ECommerce.Domain.Common;

public class Result
{
    protected Result(bool isSuccess, Error? error = null)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException("Success Result Can not has an error ");
        if(!isSuccess && error is null)
            throw new InvalidOperationException("Failure Result must has an error ");

        IsSuccess = isSuccess;
        Error = error;
    }
    public bool IsSuccess { get; }
    public Error? Error { get; }
    public bool IsFailure => !IsSuccess;

    public static Result Success() => new(true);
    public static Result Failure(Error error) => new(false,error);
    public override string ToString()
       => IsSuccess ? "Success" : $"Failure: [{Error!.Code}] {Error.Messege}";

    public static implicit operator Result(Error error) => Failure(error);

}

public sealed class Result<TValue> : Result
{
    private readonly TValue? value;

    private Result(TValue? value, bool isSuccess, Error? error = null) : base(isSuccess, error)
    {
        this.value = value;
    }

    public TValue? Value => IsSuccess
        ? value!
        : throw new InvalidOperationException("Can not access the value of a failed result.");

    public static Result<TValue> Success(TValue value) => new(value, true);
    public new static Result<TValue> Failure(Error error) => new(default, false, error);

    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure)
        => IsSuccess ? onSuccess(value!) : onFailure(Error!);

    public static implicit operator Result<TValue>(TValue value) => Success(value);
}