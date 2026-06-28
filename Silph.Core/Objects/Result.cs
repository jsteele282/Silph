using Silph.Core.Interfaces;

namespace Silph.Core.Objects
{
    public class Result : ResultBase
    {
        public Result() { }
        public Result(IMessage? message=null, IMessage? error=null, IMessage? warning=null) : base(message, error, warning) { }

        public static Result Ok(IMessage? message = null) => new(message: message);
        public static Result Fail(IMessage? error = null) => new(error: error);
        public static Result Warn(IMessage? warning = null) => new(warning: warning);
    }

    public class  Result<T> : ResultBase
    {
        public T Value { get; set; } = default!;

        public Result() { }
        public Result(T value) => Value = value;
        public Result(IMessage? message=null, IMessage? error=null, IMessage? warning=null) : base(message, error, warning) { }
        public Result(T value, IMessage? message = null, IMessage? error = null, IMessage? warning = null) : base(message, error, warning) => Value = value;

        public void AddValue(T value, IMessage? message = null, IMessage? error = null, IMessage? warning = null)
        {
            Value = value;
            if (message != null) AddMessage(message);
            if (error != null) AddError(error);
            if (warning != null) AddWarning(warning);
        }

        public static Result<T> Ok(IMessage? message = null) => new(message: message);
        public static Result<T> Fail(IMessage? error = null) => new(error: error);
        public static Result<T> Warn(IMessage? warning = null) => new(warning: warning);
                
        public static Result<T> Ok(T value, IMessage? message = null) => new(value, message: message);
        public static Result<T> Fail(T value, IMessage? error = null) => new(value, error: error);
        public static Result<T> Warn(T value, IMessage? warning = null) => new(value, warning: warning);
    }
}
