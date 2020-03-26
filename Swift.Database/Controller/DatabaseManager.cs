using NLog;
using Swift.Database.DTO.Enum;
using Swift.Database.Helper.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swift.Database.Controller
{
    public class DatabaseManager
    {
        private readonly DTO.GeneralInfo _generalInfo;
        private readonly Logger _logger;
        public DatabaseManager(DTO.GeneralInfo generalInfo)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _generalInfo = generalInfo;
        }

        private string GetReportName()
        {
            return string.Format("{0}Report_{1}.txt", _generalInfo.ScriptFolder, DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss.fff"));
        }

        public List<DTO.DbObjectDefinition> GetObjects(ObjectType objectType = ObjectType.All, string objectName = "", string schemaName = "")
        {
            List<DTO.DbObjectDefinition> result = null;

            try
            {
                Model.DatabaseManager databaseManager = new Model.DatabaseManager(_generalInfo, _generalInfo.ConnectionString);
                result = databaseManager.GetObject(objectType, objectName, schemaName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }

            return result;
        }

        public List<DTO.DbObjectDefinition> SaveObjects(List<DTO.DbObjectDefinition> dbObjectDefinitions)
        {
            List<DTO.DbObjectDefinition> result = null;

            try
            {
                Model.DatabaseManager databaseManager = new Model.DatabaseManager(_generalInfo, _generalInfo.ConnectionString);
                result = databaseManager.SaveObject(dbObjectDefinitions);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }

            return result;
        }

        public List<string> SetObjects(ObjectType objectType = ObjectType.All, string objectName = "", string schemaName = "")
        {
            List<string> result = null;

            try
            {
                Model.DatabaseManager databaseManager = new Model.DatabaseManager(_generalInfo, _generalInfo.ConnectionString);
                result = databaseManager.SetObject(objectType, objectName, schemaName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }

            return result;
        }

        public void SaveReport(List<DTO.DbObjectDefinition> dbObjectDefinitions)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("Total objects saved: {0}", dbObjectDefinitions.Count);
                sb.AppendLine();
                sb.AppendLine("Objects");

                foreach (var item in dbObjectDefinitions)
                {
                    sb.AppendLine(string.Format("{0}.{1}", item.Schema, item.Name));
                }

                File.WriteFile(GetReportName(), sb.ToString());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        public void SaveReport(List<string> objectUpdated)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(string.Format("Total objects updated: {0}", objectUpdated.Count));
                sb.AppendLine();

                sb.AppendLine("Objects:");

                foreach (var item in objectUpdated)
                {
                    sb.AppendLine(item);
                }
                sb.AppendLine();

                File.WriteFile(GetReportName(), sb.ToString());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }
    }
}
