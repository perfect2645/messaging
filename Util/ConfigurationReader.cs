using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public static class ConfigurationReader
    {
        public static string GetConfigurationValue(string key)
        {
            return Environment.GetEnvironmentVariable(key) ??
                   throw new ArgumentException($"Configuration key '{key}' not found in environment variables.");
        }

        public static string? ReadString(this IConfiguration configuration, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"ConfigurationReader::ReadString: path is null or empty.");
            }
            return configuration?[path];
        }
    }
}
