namespace Silph.Core.Interfaces;

public interface IResult
{
    IEnumerable<IMessage> Messages { get; }
    IEnumerable<IMessage> Errors { get; }
    IEnumerable<IMessage> Warnings { get; }

    bool HasMessages { get; }
    bool HasErrors { get; }
    bool HasWarnings { get; }
    bool Success { get; }
    bool Failed { get; }
}