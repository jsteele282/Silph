using Silph.Core.Interfaces;

namespace Silph.Core.Objects
{
    public abstract class ResultBase : IResult
    {
        protected List<IMessage> _messages = [];
        protected List<IMessage> _errors = [];
        protected List<IMessage> _warnings = [];

        public IEnumerable<IMessage> Messages => _messages;
        public IEnumerable<IMessage> Errors => _errors;
        public IEnumerable<IMessage> Warnings => _warnings;

        public bool HasMessages => _messages.Count > 0;
        public bool HasErrors => _errors.Count > 0;
        public bool HasWarnings => _warnings.Count > 0;

        public bool Success => !HasErrors;
        public bool Failed => HasErrors;

        protected ResultBase()
        {
        }

        protected ResultBase(IMessage? message = null, IMessage? error = null, IMessage? warning = null)
        {
            if (message != null) AddMessage(message);
            if (error != null) AddError(error);
            if (warning != null) AddWarning(warning);
        }

        public void AddMessage(IMessage message) => _messages.Add(message);
        public void AddError(IMessage error) => _errors.Add(error);
        public void AddWarning(IMessage warning) => _warnings.Add(warning);

        public void AddMessages(IEnumerable<IMessage> messages) => _messages.AddRange(messages);
        public void AddErrors(IEnumerable<IMessage> errors) => _errors.AddRange(errors);
        public void AddWarnings(IEnumerable<IMessage> warnings) => _warnings.AddRange(warnings);

        public void AddResult(IResult result)
        {
            AddMessages(result.Messages);
            AddErrors(result.Errors);
            AddWarnings(result.Warnings);
        }
    }
}
