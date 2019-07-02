using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanillaBot.Services
{
    public class ConfigService
    {
        private readonly IConfiguration _config;
        private readonly LoggingService _logger;

        public ConfigService(IConfiguration config, LoggingService logger)
        {
            _config = config;
            _logger = logger;
        }

        public T GetConfigOption<T>(string json, TryParseHandler<T> handler)
        {
            if (string.IsNullOrEmpty(_config[json]))
                return default(T);

            if (handler(_config[json], out T parsed))
                return parsed;

            return default(T);
        }

        public T GetConfigOption<T>(string json, T defaultOption, TryParseHandler<T> handler)
        {
            if (string.IsNullOrEmpty(_config[json]))
            {
                _logger.Warn("ConfigService", $"Missing config option {json}, defaulting to {defaultOption}");
                return defaultOption;
            }

            if (handler(_config[json], out T parsed))
                return parsed;

            _logger.Warn("ConfigService", $"Invalid config option {json}, defaulting to {defaultOption}");
            return defaultOption;
        }

        public delegate bool TryParseHandler<T>(string value, out T result);
    }
}
