using CK.Core;
using CK.Monitoring;

namespace CK.Glouton.Tests
{
    public static class LogEntryExtensions
    {
        /// <summary>
        /// Returns true if the logEntry text match <paramref name="message"/>.
        /// </summary>
        /// <param name="this">The log entry.</param>
        /// <param name="message">The message that the log entry needs to match.</param>
        /// <returns></returns>
        internal static bool ValidateLogEntry( this ILogEntry @this, string message )
        {
            return @this.Text == message;
        }

        /// <summary>
        /// Returns true if the logEntry text match <paramref name="message"/> and the its log level match <paramref name="logLevel"/>.
        /// </summary>
        /// <param name="this">The log entry.</param>
        /// <param name="message">The message that the log text needs to match.</param>
        /// <param name="logLevel">The log level that the log entry needs to match.</param>
        /// <returns></returns>
        internal static bool ValidateLogEntry( this ILogEntry @this, string message, LogLevel logLevel )
        {
            return @this.ValidateLogEntry( message ) && @this.LogLevel.HasFlag( logLevel );
        }
    }
}
