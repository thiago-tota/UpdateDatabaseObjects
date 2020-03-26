using Microsoft.Build.Utilities;
using Swift.Database.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Swift.Database
{
    public class DatabaseObjectManager : Task
    {
        private string _taskAction;

        public string TaskAction
        {
            get { return _taskAction; }
            set { _taskAction = value; }
        }

        private string _scriptFolder;

        public string ScriptFolder
        {
            get { return _scriptFolder; }
            set { _scriptFolder = value; }
        }

        private string _baseDirectory;

        public string BaseDirectory
        {
            get { return _baseDirectory; }
            set { _baseDirectory = value; }
        }

        private string _objectType;

        public string ObjectType
        {
            get { return _objectType; }
            set { _objectType = value; }
        }

        private string _objectName;

        public string ObjectName
        {
            get { return _objectName; }
            set { _objectName = value; }
        }

        private string _schemaName;

        public string SchemaName
        {
            get { return _schemaName; }
            set { _schemaName = value; }
        }

        private string _connectionString;

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        /// <summary>
        /// ** Mandatory fields
        /// Args -->
        ///         TaskOperation**
        ///         ScriptFolder**
        ///         ConnectionString**
        ///         ObjectType
        ///         ObjectName
        ///         SchemaName
        /// When TaskOperation == GET
        ///     ConnectionString    --> Connection string
        ///     ScriptFolder        --> Output folder
        ///     ObjectType          --> Object type option value from Swift.Database.DTO.Enum.ObjectType
        ///     ObjectName          --> Object name [optional]
        ///     SchemaName          --> Schema name [optional]
        /// When TaskOperation == SET
        ///     ConnectionString    --> Connection string
        ///     ScriptFolder        --> Output folder
        /// </summary>
        public override bool Execute()
        {
            bool result = false;

            try
            {
                GeneralInfo generalInfo = new GeneralInfo
                {
                    ConnectionString = ConnectionString,
                    ScriptFolder = ScriptFolder,
                    FileExtension = "sql",
                    BaseDirectory = BaseDirectory
                };

                switch (_taskAction)
                {
                    case "GET":
                        ExecuteTaskOne(generalInfo);
                        result = true;
                        break;
                    case "SET":
                        ExecuteTaskTwo(generalInfo);
                        result = true;
                        break;
                    default:
                        Log.LogError(string.Format(CultureInfo.CurrentCulture, "Invalid TaskAction passed: {0}", TaskAction));
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.LogError(string.Format(CultureInfo.CurrentCulture, "{0} {1}", ex.Message, ex.StackTrace));
            }

            return result;
        }

        private void ExecuteTaskOne(GeneralInfo generalInfo)
        {
            SetDefaultValues();
            Controller.DatabaseManager databaseManager = new Controller.DatabaseManager(generalInfo);
            DTO.Enum.ObjectType objectType = DTO.Enum.ObjectType.All;

            if (ObjectType != null && !ObjectType.Trim().Equals(string.Empty))
                if (!Enum.TryParse(ObjectType, true, out objectType)) objectType = DTO.Enum.ObjectType.All;

            List<DTO.DbObjectDefinition> result = databaseManager.GetObjects(objectType, ObjectName.Trim(), SchemaName.Trim());
            databaseManager.SaveObjects(result);
        }

        private void ExecuteTaskTwo(GeneralInfo generalInfo)
        {
            SetDefaultValues();
            Controller.DatabaseManager databaseManager = new Controller.DatabaseManager(generalInfo);
            DTO.Enum.ObjectType objectType = DTO.Enum.ObjectType.All; ;

            if (ObjectType != null && !ObjectType.Trim().Equals(string.Empty))
                if (!Enum.TryParse(ObjectType, true, out objectType)) objectType = DTO.Enum.ObjectType.All;

            List<string> result = databaseManager.SetObjects(objectType, ObjectName.Trim(), SchemaName.Trim());
        }

        private void SetDefaultValues()
        {
            if (ObjectType == null) ObjectType = string.Empty;
            if (ObjectName == null) ObjectName = string.Empty;
            if (SchemaName == null) SchemaName = string.Empty;
        }
    }
}
