namespace ECommerce.Domain.Common;

public sealed record Error(string Code,string Messege ,ErrorType Type)
{
    public static Error Validation(string Code, string Message)
        => new(Code, Message, ErrorType.Validation);
    public static Error NotFound(string Code, string Message)
        => new(Code, Message, ErrorType.NotFound);
    public static Error Conflict(string Code, string Message)
        => new(Code, Message, ErrorType.Conflict);
    public static Error UnAuthorized(string Code, string Message)
        => new(Code, Message, ErrorType.UnAuthorized);
    public static Error Forbidden(string Code, string Message)
        => new(Code, Message, ErrorType.Forbidden);
    public static Error Failure(string Code, string Message)
        => new(Code, Message, ErrorType.Failure);
}
