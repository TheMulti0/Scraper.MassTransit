using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Scraper.RabbitMq
{
    internal class ClassicConsoleFormatter : ConsoleFormatter
    {
        public ClassicConsoleFormatter() : base("classic")
        {
        }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            var formattedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");
            var formattedLevel = logEntry.LogLevel.ToString();
            string formattedMessage = logEntry.Formatter(logEntry.State, logEntry.Exception);

            textWriter.WriteLine(
                $"[{formattedDate}] [{formattedLevel}] [{logEntry.Category}] {formattedMessage}");
        }
    }
}