﻿using System;
using Geekbot.net.Database.LoggingAdapter;
using Geekbot.net.Lib;
using Geekbot.net.Lib.Logger;
using Microsoft.EntityFrameworkCore;
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
                    NpgsqlLogManager.Provider = new NpgsqlLoggingProviderAdapter(_logger);
                    database = new SqlDatabase(new SqlConnectionString
                    {
                        Host = _runParameters.DbHost,
                        Port = _runParameters.DbPort,
                        Database = _runParameters.DbDatabase,
                        Username = _runParameters.DbUser,
                        Password = _runParameters.DbPassword
                    });
                }
                database.Database.EnsureCreated();
                database.Database.Migrate();
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Could not Connect to datbase", e);
                Environment.Exit(GeekbotExitCode.DatabaseConnectionFailed.GetHashCode());
            }
            
            _logger.Information(LogSource.Database, $"Connected with {database.Database.ProviderName}");
            return database;
        }
    }
}