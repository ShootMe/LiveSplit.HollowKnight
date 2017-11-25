using System.Runtime.CompilerServices;

namespace LiveSplit.HollowKnight
{
    /// <summary>
    /// Abstract class that provides easy access to the logging.
    /// </summary>
    public abstract class Loggable
    {
        /// <summary>
        /// The Class's Name
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Constrcuts the class, sets the name for the logger.
        /// </summary>
        protected Loggable()
        {
            Name = GetType().Name;
        }

        /// <summary>
        /// Log at the fine/detailed level.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="methodName"></param>
        public void LogFine(string message, [CallerMemberName] string methodName = null) => Logger.Instance.LogFine(FormatLogMessage(message, methodName));

        /// <summary>
        /// Log at the debug level.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="methodName">Calling method's Name</param>
        public void LogDebug(string message, [CallerMemberName] string methodName = null) => Logger.Instance.LogDebug(FormatLogMessage(message, methodName));

        /// <summary>
        /// Log at the info level.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="methodName">Calling method's Name</param>
        public void Log(string message, [CallerMemberName] string methodName = null) => Logger.Instance.Log(FormatLogMessage(message, methodName));

        /// <summary>
        /// Log at the warn level.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="methodName">Calling method's Name</param>
        public void LogWarn(string message, [CallerMemberName] string methodName = null) => Logger.Instance.LogWarn(FormatLogMessage(message, methodName));

        /// <summary>
        /// Log at the error level.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="methodName">Calling method's Name</param>
        public void LogError(string message, [CallerMemberName] string methodName = null) => Logger.Instance.LogError(FormatLogMessage(message, methodName));

        /// <summary>
        /// Formats a log message as "[TypeName] - Message"
        /// </summary>
        /// <param name="message">Message to be formatted.</param>
        /// <param name="methodName">Calling method's Name</param>
        /// <returns>Formatted Message</returns>
        private string FormatLogMessage(string message,string methodName) => $"[{Name}.{methodName}] - {message}";

    }
}
