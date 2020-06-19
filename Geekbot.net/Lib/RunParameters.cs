using System;
using CommandLine;

namespace Geekbot.net.Lib
{
    public class RunParameters
    {
        /************************************
         * General                          *
         ************************************/
        
        [Option("token", HelpText = "Set a new bot token. By default it will use your previous bot token which was stored in the database (default: null) (env: TOKEN)")]
        public string Token { get; set; } = ParamFallback("TOKEN");

        [Option('V', "verbose", HelpText = "Logs everything. (default: false) (env: LOG_VERBOSE)")]
        public bool Verbose { get; set; } = ParamFallback("LOG_VERBOSE", false);

        [Option('j', "log-json", HelpText = "Logger outputs json (default: false ) (env: LOG_JSON)")]
        public bool LogJson { get; set; } = ParamFallback("LOG_JSON", false);

        [Option('e', "expose-errors", HelpText = "Shows internal errors in the chat (default: false) (env: EXPOSE_ERRORS)")]
        public bool ExposeErrors { get; set; } = ParamFallback("EXPOSE_ERRORS", false);

        /************************************
         * Database                         *
         ************************************/
        
        [Option("in-memory", HelpText = "Uses the in-memory database instead of postgresql (default: false) (env: DB_INMEMORY)")]
        public bool InMemory { get; set; } = ParamFallback("DB_INMEMORY", false);
        
        // Postresql connection
        [Option("database", HelpText = "Select a postgresql database (default: geekbot) (env: DB_DATABASE)")]
        public string DbDatabase { get; set; } = ParamFallback("DB_DATABASE", "geekbot");

        [Option("db-host", HelpText = "Set a postgresql host (default: localhost) (env: DB_HOST)")]
        public string DbHost { get; set; } = ParamFallback("DB_HOST", "localhost");
        
        [Option("db-port", HelpText = "Set a postgresql host (default: 5432) (env: DB_PORT)")]
        public string DbPort { get; set; } = ParamFallback("DB_PORT", "5432");

        [Option("db-user", HelpText = "Set a postgresql user (default: geekbot) (env: DB_USER)")]
        public string DbUser { get; set; } = ParamFallback("DB_USER", "geekbot");

        [Option("db-password", HelpText = "Set a posgresql password (default: empty) (env: DB_PASSWORD)")]
        public string DbPassword { get; set; } = ParamFallback("DB_PASSWORD", "");

        [Option("db-require-ssl", HelpText = "Require SSL to connect to the database (default: false) (env: DB_REQUIRE_SSL)")]
        public bool DbSsl { get; set; } = ParamFallback("DB_REQUIRE_SSL", false);
        
        [Option("db-trust-cert", HelpText = "Trust the database certificate, regardless if it is valid (default: false) (env: DB_TRUST_CERT)")]
        public bool DbTrustCert { get; set; } = ParamFallback("DB_TRUST_CERT", false);
        
        // Logging
        [Option("db-logging", HelpText = "Enable database logging (default: false) (env: DB_LOGGING)")]
        public bool DbLogging { get; set; } = ParamFallback("DB_LOGGING", false);

        /************************************
         * WebApi                           *
         ************************************/
        
        [Option('a', "disable-api", HelpText = "Disables the WebApi (default: false) (env: API_DISABLE)")]
        public bool DisableApi { get; set; } = ParamFallback("API_DISABLE", false);

        [Option("api-host", HelpText = "Host on which the WebApi listens (default: localhost) (env: API_HOST)")]
        public string ApiHost { get; set; } = ParamFallback("API_HOST", "localhost");
        
        [Option("api-port", HelpText = "Port on which the WebApi listens (default: 12995) (env: API_PORT)")]
        public string ApiPort { get; set; } = ParamFallback("API_PORT", "12995");
        
        /************************************
         * Intergrations                    *
         ************************************/

        [Option("sumologic", HelpText = "Sumologic endpoint for logging (default: null) (env: SUMOLOGIC)")]
        public string SumologicEndpoint { get; set; } = ParamFallback("SUMOLOGIC");
        
        [Option("sentry", HelpText = "Sentry endpoint for error reporting (default: null) (env: SENTRY)")]
        public string SentryEndpoint { get; set; } = ParamFallback("SENTRY");
        
        /************************************
         * Helper Functions                 *
         ************************************/
        
        private static string ParamFallback(string key, string defaultValue = null)
        {
            var envVar = GetEnvironmentVariable(key);
            return !string.IsNullOrEmpty(envVar) ? envVar : defaultValue;
        }
        
        private static bool ParamFallback(string key, bool defaultValue)
        {
            var envVar = GetEnvironmentVariable(key);
            if (!string.IsNullOrEmpty(envVar))
            {
                return envVar.ToLower() switch
                {
                    "true" => true,
                    "1" => true,
                    "false" => false,
                    "0" => false,
                    _ => defaultValue
                };
            }

            return defaultValue;
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable($"GEEKBOT_{name}");
        }
    }
}