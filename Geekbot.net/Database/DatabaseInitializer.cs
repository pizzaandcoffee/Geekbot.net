using System;
using Geekbot.net.Lib;
using Geekbot.net.Lib.Logger;

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
        }

        public DatabaseContext Initzialize()
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
                    database = new SqlDatabase(new SqlConnectionString());
                }

                database.Database.EnsureCreated();
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Could not Connect to datbase", e);
                Environment.Exit(GeekbotExitCode.DatabaseConnectionFailed.GetHashCode());
            }

            return database;
        }
    }
}