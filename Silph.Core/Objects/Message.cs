using Silph.Core.Interfaces;
using System.Reflection;

namespace Silph.Core.Objects
{
    public class Message : IMessage
    {
        /// <summary>
        /// Intended to be a user-friendly title for the message when displayed in the UI.  Not required for logging or internal use.  Default set by object, caller determines override string.
        /// </summary>
        public string Title { get; init; }

        /// <summary>
        /// The MessageCode associated with this message, used for programmatic handling and logging.  Should be unique across the project and ideally across the organization.  See MessageCodes for standard codes and organization.
        /// </summary>
        public int Code { get; init; }

        /// <summary>
        /// The template string for the message.  Formatting options should be used for any variable content, and the template should be written with the expectation that it may be used for both user-facing messages and internal logging.
        /// </summary>
        public string Template { get; init; }

        /// <summary>
        /// The message type, determining categorization and default logging behavior.  See MessageType for details on each type.
        /// </summary>
        public MessageType Type { get; init; }

        /// <summary>
        /// Allows an override to be set for the log scope of this message.
        /// </summary>
        public LogScope? LogScopeOverride { get; init; }


        public Message(int code, string template, MessageType type, LogScope? logScopeOverride = null, string? title = null)
        {
            Code = code;
            Title = title ?? type.ToString();
            Template = template;
            Type = type;
            LogScopeOverride = logScopeOverride;
        }

        public string Format(params object[] args)
        {
            return args.Length == 0 ? Template : string.Format(Template, args);
        }

        public override string ToString()
        {
            return Template;
        }

        public LogScope LogScope
        {
            get
            {
                if (LogScopeOverride.HasValue)
                {
                    return LogScopeOverride.Value;
                }
                return Type switch
                {
                    MessageType.Debug => LogScope.None,
                    MessageType.Info => LogScope.None,
                    MessageType.Warning => LogScope.None,
                    MessageType.Success => LogScope.Structured,
                    MessageType.Error => LogScope.Structured,
                    MessageType.Critical => LogScope.Bootstrap,
                    _ => LogScope.None
                };
            }
        }
    }
}
