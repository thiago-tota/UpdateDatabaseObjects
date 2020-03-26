using Microsoft.Extensions.Configuration;
using NLog;
using Swift.Database.DTO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Swift.Database.View
{
    public class Program
    {
        /// <summary>
        /// Args
        /// When args[0] == GET
        ///     args[1] --> Connection string
        ///     args[2] --> Output folder
        ///     args[3] --> Object name [optional]
        ///     args[4] --> Schema name [optional]
        ///     args[5] --> Object type option value from Swift.Database.DTO.Enum.ObjectType
        /// When args[0] == SET
        ///    args[1] --> Connection string
        ///    args[2] --> Input folder
        ///    args[3] --> Object name [optional]
        ///    args[4] --> Schema name [optional]
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            if (args.Length < 3)
                ThrowArgumentException();
            else
            {
                GeneralInfo generalInfo = InitializeGeneralSettings(args);

                switch (args[0].Trim())
                {
                    case "GET":
                        ExecuteTaskOne(generalInfo, args);
                        break;
                    case "SET":
                        ExecuteTaskTwo(generalInfo, args);
                        break;
                    default:
                        ThrowInvalidArgumentException(args[0].Trim());
                        break;
                }
            }
        }

        private static GeneralInfo InitializeGeneralSettings(string[] args)
        {
            LogManager.ReconfigExistingLoggers();

            IConfigurationBuilder builder = new ConfigurationBuilder()
                                                    .SetBasePath(Directory.GetCurrentDirectory())
                                                    .AddJsonFile("appSettings.json");

            IConfigurationRoot configuration = builder.Build();

            return new GeneralInfo()
            {
                FileExtension = configuration["fileExtension"],
                ConnectionString = args[1],
                ScriptFolder = args[2]
            };
        }

        private static void ExecuteTaskOne(GeneralInfo generalInfo, string[] args)
        {
            Controller.DatabaseManager databaseManager = new Controller.DatabaseManager(generalInfo);
            DTO.Enum.ObjectType objectType = DTO.Enum.ObjectType.All;
            string objectName = string.Empty;
            string schemaName = string.Empty;

            if (args.Length > 3)
                objectName = args[3];

            if (args.Length > 4)
                schemaName = args[4];

            if (args.Length > 5)
                objectType = (DTO.Enum.ObjectType)Enum.Parse(typeof(DTO.Enum.ObjectType), args[5]);

            List<DbObjectDefinition> result = databaseManager.SaveObjects(databaseManager.GetObjects(objectType, objectName, schemaName));

            if (generalInfo.GenerateReport)
                databaseManager.SaveReport(result);
        }

        private static void ExecuteTaskTwo(GeneralInfo generalInfo, string[] args)
        {
            Controller.DatabaseManager databaseManager = new Controller.DatabaseManager(generalInfo);
            DTO.Enum.ObjectType objectType = DTO.Enum.ObjectType.All;
            string objectName = string.Empty;
            string schemaName = string.Empty;

            if (args.Length > 3)
                objectName = args[3];

            if (args.Length > 4)
                schemaName = args[4];

            if (args.Length > 5)
                objectType = (DTO.Enum.ObjectType)Enum.Parse(typeof(DTO.Enum.ObjectType), args[5]);

            List<string> result = databaseManager.SetObjects(objectType, objectName, schemaName);

            if (generalInfo.GenerateReport)
                databaseManager.SaveReport(result);
        }

        private static void ThrowArgumentException()
        {
            throw new ArgumentException("Wrong number of parameters.");
        }

        private static void ThrowInvalidArgumentException(string argument)
        {
            throw new ArgumentException(string.Format("Argument {0} is invalid.", argument));
        }
    }
}
