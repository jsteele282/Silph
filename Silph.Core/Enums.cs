namespace Silph.Core
{
    public enum MessageType
    {
        /// <summary>
        /// The action completed as expected.  Logged to structured logs by default.
        /// </summary>
        Success = 0,

        /// <summary>
        /// A non-structural failure.  Logged to structured logs by default.
        /// </summary>
        Error = 1,

        /// <summary>
        /// A structural workflow or application failure.  Logged to bootstrap and structured logs by default.
        /// </summary>
        Critical = 2,

        /// <summary>
        /// User-facing context; not logged by default.
        /// </summary>
        Info = 3,

        /// <summary>
        /// User-facing caution that does not represent a failure.  Not logged by default.
        /// </summary>
        Warning = 4,

        /// <summary>
        /// Developer/Admin diagnostic information.  Not logged by default unless a scope override is set.
        /// </summary>
        Debug = 5,
    }

    public enum  LogScope
    {
        /// <summary>
        /// User-facing message; no log will be created.
        /// </summary>
        None,

        /// <summary>
        /// Log will only be created if database access is available; messages are intended for structured logging and user display.
        /// </summary>
        Structured,

        /// <summary>
        /// Logs will be created regardless; messages will be tracked in project-level text file and, if available, database logging.
        /// </summary>
        Bootstrap
    }
}
