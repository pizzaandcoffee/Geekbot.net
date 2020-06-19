using System;
using Geekbot.net.Database.LoggingAdapter;
using Geekbot.net.Lib;
using Geekbot.net.Lib.Logger;
using Npgsql.Logging;

namespace Geekbot.net.Database
{
    public class DatabaseInitializer
    {
        private readonly RunParameters _runParameters;
        private readonly GeekbotLogger _logger;

        public DatabaseInitializer(RunParameters runParameters, GeekbotLogger logger)
        {
            _runParameters = runParameters;
            _logger = logger;
            NpgsqlLogManager.Provider = new NpgsqlLoggingProviderAdapter(logger, runParameters);
        }

        public DatabaseContext Initialize()
        {
            DatabaseContext database = null;
            try
            {
                if (_runParameters.InMemory)
                {
                    database = new InMemoryDatabase("geekbot");
                }
                else
                {
                    database = new SqlDatabase(new SqlConnectionString
                    {
                        Host = _runParameters.DbHost,
                        Port = _runParameters.DbPort,
                        Database = _runParameters.DbDatabase,
                        Username = _runParameters.DbUser,
                        Password = _runParameters.DbPassword,
                        RequireSsl = _runParameters.DbSsl,
                        TrustServerCertificate = _runParameters.DbTrustCert
                    });
                }
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Could not Connect to datbase", e);
                Environment.Exit(GeekbotExitCode.DatabaseConnectionFailed.GetHashCode());
            }

            if (_runParameters.DbLogging)
            {
                _logger.Information(LogSource.Database, $"Connected with {database.Database.ProviderName}");
            }

            return database;
        }
    }
}