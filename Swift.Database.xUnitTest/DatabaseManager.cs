using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Swift.Database.xUnitTest
{
    public class DatabaseManager
    {
        readonly int _limitList = 10000;
        readonly DTO.GeneralInfo _generalInfo;

        public DatabaseManager()
        {
            _generalInfo = new DTO.GeneralInfo();

            IConfigurationBuilder builder = new ConfigurationBuilder()
                                                    .SetBasePath(Directory.GetCurrentDirectory())
                                                    .AddJsonFile("appSettings.json");

            IConfigurationRoot configuration = builder.Build();

            _generalInfo.ScriptFolder = configuration["scriptFolder"];
            _generalInfo.FileExtension = configuration["fileExtension"];
            _generalInfo.ConnectionString = configuration["connectionString"];

            _limitList = Convert.ToInt32(configuration["limitListForTest"]);
        }

        [Fact, Order(1)]
        public void ExtractProceduresFromDatabase()
        {
            Controller.DatabaseManager databaseManager = new Controller.DatabaseManager(_generalInfo);
            Assert.True(databaseManager.GetObjects(DTO.Enum.ObjectType.Procedures).Count > 0);
        }

        [Fact, Order(1)]
        public void ExtractFunctionsFromDatabase()
        {

            Controller.DatabaseManager databaseManager = new Controller.DatabaseManager(_generalInfo);
            Assert.True(databaseManager.GetObjects(DTO.Enum.ObjectType.Functions).Count > 0);
        }

        [Fact, Order(1)]
        public void ExtractViewsFromDatabase()
        {
            Controller.DatabaseManager databaseManager = new Controller.DatabaseManager(_generalInfo);
            Assert.True(databaseManager.GetObjects(DTO.Enum.ObjectType.Views).Count > 0);
        }

        [Fact, Order(2)]
        public void SaveDatabaseObjectsToFile()
        {
            Controller.DatabaseManager databaseManager = new Controller.DatabaseManager(_generalInfo);
            List<DTO.DbObjectDefinition> items = databaseManager.GetObjects().Take(_limitList).ToList();
            Assert.Null(Record.Exception(() => databaseManager.SaveObjects(items)));
        }

        [Fact, Order(3)]
        public void SaveDatabaseProceduresToFile()
        {
            Controller.DatabaseManager databaseManager = new Controller.DatabaseManager(_generalInfo);
            List<DTO.DbObjectDefinition> items = databaseManager.GetObjects(DTO.Enum.ObjectType.Procedures).Take(_limitList).ToList();
            Assert.Null(Record.Exception(() => databaseManager.SaveObjects(items)));
        }

        [Fact, Order(3)]
        public void SaveDatabaseFunctionsToFile()
        {
            Controller.DatabaseManager databaseManager = new Controller.DatabaseManager(_generalInfo);
            List<DTO.DbObjectDefinition> items = databaseManager.GetObjects(DTO.Enum.ObjectType.Functions).Take(_limitList).ToList();
            Assert.Null(Record.Exception(() => databaseManager.SaveObjects(items)));
        }

        [Fact, Order(3)]
        public void SaveDatabaseViewsToFile()
        {
            Controller.DatabaseManager databaseManager = new Controller.DatabaseManager(_generalInfo);
            List<DTO.DbObjectDefinition> items = databaseManager.GetObjects(DTO.Enum.ObjectType.Views).Take(_limitList).ToList();
            Assert.Null(Record.Exception(() => databaseManager.SaveObjects(items)));
        }

        [Fact, Order(3)]
        public void SaveDatabaseTriggersToFile()
        {
            Controller.DatabaseManager databaseManager = new Controller.DatabaseManager(_generalInfo);
            List<DTO.DbObjectDefinition> items = databaseManager.GetObjects(DTO.Enum.ObjectType.Triggers).Take(_limitList).ToList();
            Assert.Null(Record.Exception(() => databaseManager.SaveObjects(items)));
        }

        [Fact, Order(4)]
        public void UpdateDatabaseViews()
        {
            Controller.DatabaseManager databaseManager = new Controller.DatabaseManager(_generalInfo);
            List<string> items = databaseManager.SetObjects(DTO.Enum.ObjectType.Views);
            Assert.NotNull(items);
        }

        [Fact, Order(5)]
        public void UpdateDatabaseObject()
        {
            Controller.DatabaseManager databaseManager = new Controller.DatabaseManager(_generalInfo);
            List<string> items = databaseManager.SetObjects(DTO.Enum.ObjectType.All, "vwa_EZoneAirwayBills");
            Assert.NotNull(items);
        }

        [Fact, Order(6)]
        public void UpdateDatabase()
        {
            Controller.DatabaseManager databaseManager = new Controller.DatabaseManager(_generalInfo);
            List<string> items = databaseManager.SetObjects();
            Assert.NotNull(items);
        }

        [Fact, Order(7)]
        private void ClearFiles()
        {
            //Last test only to clear files
            Helper.Generic.File.DeleteManyFile(_generalInfo.ScriptFolder, _generalInfo.FileExtension.Contains('.')
                        ? _generalInfo.FileExtension : string.Format("*.{0}", _generalInfo.FileExtension));
        }
    }
}
