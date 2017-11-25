using System;
using System.IO;
using System.Text;

namespace LiveSplit.HollowKnight
{

    /// <summary>
    /// Shared logger for mods to use.
    /// </summary>
    // This is threadsafe, but it's blocking.  Hopefully mods don't try to log so much that it becomes an issue.  If it does we'll have to look at a better system.
    public class Logger
    {
        private static readonly object Locker = new object();

        private readonly LogLevel _logLevel;

        private static Logger _instance;

        public static Logger Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = new Logger(LogLevel.Debug, "_HollowKnight.log");
                return _instance;
            }
        }

        /// <summary>
        /// Logger Constructor.  Initializes file to write to.
        /// </summary>
        /// <param name="loglevel"></param>
        /// <param name="path"></param>
        public Logger(LogLevel loglevel, string path)
        {
            if (!File.Exists(path)) return;

            _logLevel = loglevel;

            FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            _writer = new StreamWriter(fileStream, Encoding.UTF8) { AutoFlush = true };
        }

        /// <summary>
        /// Checks to ensure that the logger level is currently high enough for this message, if it is, write it.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="level">Level of Log</param>
        private void Log(string message, LogLevel level)
        {
            if (_logLevel <= level)
                WriteToFile("[" + level.ToString().ToUpper() + "]:" + message + Environment.NewLine);
        }

        /// <summary>
        /// Finest/Lowest level of logging.  Usually reserved for developmental testing.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void LogFine(string message) => Log(message, LogLevel.Fine);

        /// <summary>
        /// Log at the debug level.  Usually reserved for diagnostics.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void LogDebug(string message) => Log(message, LogLevel.Debug);

        /// <summary>
        /// Log at the info level.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Log(string message) => Log(message, LogLevel.Info);

        /// <summary>
        /// Log at the warning level.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void LogWarn(string message) => Log(message, LogLevel.Warn);

        /// <summary>
        /// Log at the error level.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void LogError(string message) => Log(message, LogLevel.Error);

        /// <summary>
        /// Locks file to write, writes to file, releases lock.
        /// </summary>
        /// <param name="text">Text to write</param>
        private void WriteToFile(string text)
        {
            if (_writer == null) return;

            lock (Locker)
            {
                _writer.Write(text);
            }
        }

        private readonly StreamWriter _writer;
    }

    /// <summary>
    /// What level should logs be done at?
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Finest Level of Logging - Developers Only
        /// </summary>
        Fine,
        /// <summary>
        /// Debug Level of Logging - Mostly Developers Only
        /// </summary>
        Debug,
        /// <summary>
        /// Normal Logging Level
        /// </summary>
        Info,
        /// <summary>
        /// Only Show Warnings and Above
        /// </summary>
        Warn,
        /// <summary>
        /// Only Show Full Errors
        /// </summary>
        Error,
        /// <summary>
        /// No Logging at all
        /// </summary>
        Off
    }
}
