using System;
using System.Configuration;

namespace SolviaCopyFilesAtLogon
{
    public static class ConfigHelper
    {
        public static string GetSetting(string key) => ConfigurationManager.AppSettings[key] ?? string.Empty;

        public static int GetIntSetting(string key) => int.TryParse(GetSetting(key), out int value) ? value : 0;

        public static bool GetBoolSetting(string key) => bool.TryParse(GetSetting(key), out bool value) && value;

        public static string GetExpandedSetting(string key) => Environment.ExpandEnvironmentVariables(GetSetting(key));
    }
}
