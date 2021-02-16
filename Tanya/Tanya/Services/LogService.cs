using Discord;
using System;
using System.Threading.Tasks;

namespace Tanya.Services
{
    public static class LogService
    {
        public static async Task LogAsync(string src, LogSeverity severity, string message)
        {
            if (severity.Equals(null))
            {
                severity = LogSeverity.Warning;
            }
            await Append($"{SeverityToString(severity)}", GetConsoleColor(severity));
            await Append($" [{SourceToString(src)}] ", ConsoleColor.DarkGray);

            if (string.IsNullOrWhiteSpace(message))
                return;
            await Append($"{message}\n", ConsoleColor.White);
        }

        public static async Task LogCritAsync(string source, string message)
            => await LogAsync(source, LogSeverity.Critical, message);

        public static async Task LogInfoAsync(string source, string message)
            => await LogAsync(source, LogSeverity.Info, message);

        private static async Task Append(string message, ConsoleColor color)
        {
            await Task.Run(() => {
                Console.ForegroundColor = color;
                Console.Write(message);
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
