using Discord;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace Yuna.Services
{
    public static class LogService
    {
        public static async Task LogAsync(string src, LogSeverity severity, string message, Exception exception = null)
        {
            if (severity.Equals(null))
            {
                severity = LogSeverity.Warning;
            }
            
            if (!string.IsNullOrWhiteSpace(message))
                await Append($"{SeverityToString(severity)}", $" [{SourceToString(src)}] ", $"{message}\n", GetConsoleColor(severity));
            else if (exception == null)
            {
                await Append($"{SeverityToString(severity)}", $" [{SourceToString(src)}] ", "Uknown Exception. Exception Returned Null.\n", ConsoleColor.DarkRed);
            }
            else if (exception.Message == null)
                await Append($"{SeverityToString(severity)}", $" [{SourceToString(src)}] ", $"Unknownk \n{exception.StackTrace}\n", GetConsoleColor(severity));
            else
                await Append($"{SeverityToString(severity)}", $" [{SourceToString(src)}] ", $"{exception.Message ?? "Unknownk"}\n{exception.StackTrace ?? "Unknown"}\n", GetConsoleColor(severity));

        }
        // Used whenever we want to log something critical to the Console in the code. 
        public static async Task LogCritAsync(string source, string message)
            => await LogAsync (source, LogSeverity.Critical, message);
        // Used whenever we want to log some information to the Console in the code. 
        public static async Task LogInfoAsync(string source, string message)
            => await LogAsync(source, LogSeverity.Info, message);
        private static async Task Append(string severity, string source, string message, ConsoleColor color)
        {
            await Task.Run(() => {
                Console.ForegroundColor = color;
                Console.Write(severity + source + message);
            });
        }

        // The Normal Source Input To Something Neater
        private static string SourceToString(string src)
        {
            return src.ToLower() switch
            {
                "discord" => "DISCD",
                "victoria" => "VICTR",
                "music" => "MUSIC",
                "admin" => "ADMIN",
                "gateway" => "GTWAY",
                "blacklist" => "BLAKL",
                "lavanode_0_socket" => "LAVAS",
                "lavanode_0" => "LAVA#",
                "bot" => "BOTWN",
                _ => src,
            };
        }

        // Swap The Severity To a String
        private static string SeverityToString(LogSeverity severity)
        {
            return severity switch
            {
                LogSeverity.Critical => "CRIT",
                LogSeverity.Debug => "DBUG",
                LogSeverity.Error => "EROR",
                LogSeverity.Info => "INFO",
                LogSeverity.Verbose => "VERB",
                LogSeverity.Warning => "WARN",
                _ => "UNKN",
            };
        }

        // Returns The Console Color
        private static ConsoleColor GetConsoleColor(LogSeverity severity)
        {
            return severity switch
            {
                LogSeverity.Critical => ConsoleColor.Red,
                LogSeverity.Debug => ConsoleColor.DarkMagenta,
                LogSeverity.Error => ConsoleColor.DarkRed,
                LogSeverity.Info => ConsoleColor.DarkGray,
                LogSeverity.Verbose => ConsoleColor.DarkCyan,
                LogSeverity.Warning => ConsoleColor.DarkYellow,
                _ => ConsoleColor.White,
            };
        }
    }
}
