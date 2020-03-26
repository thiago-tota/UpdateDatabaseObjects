using Microsoft.Extensions.Configuration;
using Swift.Database.Helper;
using System.IO;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Swift.Database.xUnitTest
{
    public class ConsoleApplication
    {
        readonly DTO.GeneralInfo _generalInfo;

        public ConsoleApplication()
        {
            _generalInfo = new DTO.GeneralInfo();

            IConfigurationBuilder builder = new ConfigurationBuilder()
                                                    .SetBasePath(Directory.GetCurrentDirectory())
                                                    .AddJsonFile("appSettings.json");

            IConfigurationRoot configuration = builder.Build();

            _generalInfo.ScriptFolder = configuration["scriptFolder"];
            _generalInfo.FileExtension = configuration["fileExtension"];
            _generalInfo.ConnectionString = configuration["connectionString"];
        }

        [Fact, Order(1)]
        public void Task1NoFilter()
        {
            string[] args =
                new string[]{
                            "GET",
                            _generalInfo.ConnectionString,
                            _generalInfo.ScriptFolder
                };

            Assert.Null(Record.Exception(() => View.Program.Main(args)));
        }

        [Fact, Order(2)]
        public void Task1FilterProcedures()
        {
            string[] args =
                new string[]{
                            "GET",
                            _generalInfo.ConnectionString,
                            _generalInfo.ScriptFolder,
                            string.Empty, string.Empty, "Procedures"
                };

            Assert.Null(Record.Exception(() => View.Program.Main(args)));
        }

        [Fact, Order(2)]
        public void Task1FilterView()
        {
            string[] args =
                new string[]{
                            "GET",
                            _generalInfo.ConnectionString,
                            _generalInfo.ScriptFolder,
                            string.Empty,  "internal", "Views"
                };

            Assert.Null(Record.Exception(() => View.Program.Main(args)));
        }

        [Fact, Order(2)]
        public void Task1FilterTriggers()
        {
            string[] args =
                new string[]{
                            "GET",
                            _generalInfo.ConnectionString,
                            _generalInfo.ScriptFolder,
                            string.Empty, string.Empty, "Views"
                };

            Assert.Null(Record.Exception(() => View.Program.Main(args)));
        }

        [Fact, Order(2)]
        public void Task1FilterName()
        {
            string[] args =
                new string[]{
                            "GET",
                            _generalInfo.ConnectionString,
                            _generalInfo.ScriptFolder,
                            "spA_API_ParcelLockers_ParcelDelivered",
                            string.Empty
                };

            Assert.Null(Record.Exception(() => View.Program.Main(args)));
        }

        [Fact, Order(3)]
        public void Task1FilterSchema()
        {
            string[] args =
                new string[]{
                            "GET",
                            _generalInfo.ConnectionString,
                            _generalInfo.ScriptFolder,
                            string.Empty, "addr"
                };

            Assert.Null(Record.Exception(() => View.Program.Main(args)));
        }

        [Fact, Order(4)]
        public void Task2NoFilter()
        {
            string[] args =
                new string[]{
                            "SET",
                            _generalInfo.ConnectionString,
                            _generalInfo.ScriptFolder
                };

            Assert.Null(Record.Exception(() => View.Program.Main(args)));
        }

        [Fact, Order(4)]
        public void Task2FilterName()
        {
            string[] args =
                new string[]{
                            "SET",
                            _generalInfo.ConnectionString,
                            _generalInfo.ScriptFolder,
                            "spA_CheckPayments"
                };

            Assert.Null(Record.Exception(() => View.Program.Main(args)));
        }

        [Fact, Order(4)]
        public void Task2FilterSchema()
        {
            string[] args =
                new string[]{
                            "SET",
                            _generalInfo.ConnectionString,
                            _generalInfo.ScriptFolder,
                            string.Empty, "internal"
                };

            Assert.Null(Record.Exception(() => View.Program.Main(args)));
        }

        [Fact, Order(5)]
        private void ClearFiles()
        {
            //Last test only to clear files
            Helper.Generic.File.DeleteManyFile(_generalInfo.ScriptFolder, _generalInfo.FileExtension.Contains('.')
                        ? _generalInfo.FileExtension : string.Format("*.{0}", _generalInfo.FileExtension));
        }
    }
}
