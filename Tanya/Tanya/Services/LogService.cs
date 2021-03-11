using Discord;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tanya.Services
{
    public static class LogService
    {
        private static readonly Semaphore semaphore = new Semaphore(1, 1);
        public static Task LogAsync(string src, LogSeverity severity, string message)
        {
            semaphore.WaitOne();
            if (severity.Equals(null))
            {
                severity = LogSeverity.Warning;
            }
            Task task;
            task = Append($"{SeverityToString(severity)}", $" [{SourceToString(src)}] ", $"{message}\n", GetConsoleColor(severity));
            if (string.IsNullOrWhiteSpace(message))
                return task;
            semaphore.Release();

            return task;
        }

        public static Task LogCritAsync(string source, string message)
            => LogAsync(source, LogSeverity.Critical, message);

        public static Task LogInfoAsync(string source, string message)
            => LogAsync(source, LogSeverity.Info, message);
        private static Task Append(string severity, string source, string message, ConsoleColor color)
        {
            return Task.Run(() => {
                Console.ForegroundColor = color;
                Console.Write(severity + source + message);
            });
        }

        // The Normal Source Input To Something Neater
        private static string SourceToString(string src)
        {
            return (src.ToLower()) switch
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
                LogSeverity.Debug => ConsoleColor.Magenta,
                LogSeverity.Error => ConsoleColor.DarkRed,
                LogSeverity.Info => ConsoleColor.Green,
                LogSeverity.Verbose => ConsoleColor.DarkCyan,
                LogSeverity.Warning => ConsoleColor.Yellow,
                _ => ConsoleColor.White,
            };
        }
    }
}
