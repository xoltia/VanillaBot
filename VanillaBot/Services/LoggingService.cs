using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanillaBot.Services
{
    public class LoggingService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        private string _logDirectory => Path.Combine(AppContext.BaseDirectory, "logs");
        private string _logFile => Path.Combine(_logDirectory, $"{DateTime.Now.ToString("mm-dd-yyyy")}.txt");

        public LoggingService(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
        }

        private Task Log(LogMessage message)
        {
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
            if (!File.Exists(_logFile))
                File.Create(_logFile).Dispose();

            string logMessage = $"{DateTime.Now.ToString("hh:mm:ss")} [{message.Severity}] {message.Source}: {message.Exception?.ToString() ?? message.Message}";
            File.AppendAllText(_logFile, logMessage + "\n");
            return Console.Out.WriteLineAsync(logMessage);
        }

        public Task Initialize()
        {
            _client.Log += Log;
            _commands.Log += Log;
            return Task.CompletedTask;
        }
    }
}
