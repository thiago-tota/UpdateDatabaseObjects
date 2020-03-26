using NLog;
using Swift.Database.DTO.Enum;
using Swift.Database.Helper.Generic;
using Swift.Database.Model.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace Swift.Database.Model
{
    public class DatabaseManager : DataBaseAccess
    {
        const string CREATE_INDEX = "CREATE_INDEX_RESERVED_WORD";

        private readonly DTO.GeneralInfo _generalInfo;
        private readonly Logger _logger;
        private readonly Lazy<Regex> _definitionNameRegex = new Lazy<Regex>(() => new Regex(@"^\s*(((CREATE)|(ALTER)) +((PROC(EDURE)?)|(VIEW)|(TRIGGER)|(FUNCTION)))\s+(((?<Name>(\w|\.|-)+)|(\[|\])?)+)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Multiline));

        public DatabaseManager(DTO.GeneralInfo generalInfo, string connectionString) : base(connectionString)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _generalInfo = generalInfo;
        }

        public List<DTO.DbObjectDefinition> GetObject(ObjectType objectType = ObjectType.All, string objectName = "", string schemaName = "")
        {
            List<DbParameter> parameters = new List<DbParameter>();
            List<string> objectTypes;
            List<DTO.DbObjectDefinition> dbObjectDefinitions = new List<DTO.DbObjectDefinition>();

            try
            {
                objectTypes = objectType.GetValue();
                foreach (string item in objectTypes)
                {
                    #region Parameters
                    //Parameters need to be recreated every time otherwise, 
                    //an exception ("The SqlParameter is already contained by another SqlParameterCollection.") will be thrown.
                    parameters.Clear();
                    parameters.Add(GetParameter("@ObjectName", string.IsNullOrEmpty(objectName) ? null : objectName));
                    parameters.Add(GetParameter("@SchemaName", string.IsNullOrEmpty(schemaName) ? null : schemaName));

                    if (objectType != ObjectType.All)
                        parameters.Add(GetParameter("@ObjectType", item));
                    else
                        parameters.Add(GetParameter("@ObjectType", null));

                    #endregion
                    using (DbDataReader dr = ExecuteDataReader(_generalInfo.GetObjectDefinition, parameters, CommandType.Text))
                    {
                        DTO.DbObjectDefinition dbObjectDefinition;
                        while (dr.Read())
                        {
                            dbObjectDefinition = new DTO.DbObjectDefinition(dr);

                            if (ObjectType.Views.GetValue().Contains(dbObjectDefinition.Type))
                            {
                                dbObjectDefinition.Definition += GetIndexFromView(dbObjectDefinition.Name);
                            }

                            dbObjectDefinitions.Add(dbObjectDefinition);
                        }
                    }

                    if (objectType == ObjectType.All)
                        break;
                }
            }
            catch (Exception ex)
            {
                LogManager.ReconfigExistingLoggers();
                _logger.Error(ex);
                throw;
            }

            return dbObjectDefinitions;
        }

        private bool CompareNameToDefinitionName(string name, string definition)
        {
            var definitionName = GetDefinitionName(definition);

            bool warned = WarnIfDefinitionMismatch(name, definitionName);
            return !warned;
        }

        private bool CompareNameToDefinitionName(string name, IEnumerable<string> definitionLines)
        {
            var definitionName = GetDefinitionName(definitionLines);

            bool warned = WarnIfDefinitionMismatch(name, definitionName);
            return !warned;
        }

        private string GetDefinitionName(string definition)
        {
            var match = _definitionNameRegex.Value.Matches(definition).Cast<Match>().SingleOrDefault();

            if (match == null)
            {
                return null;
            }

            return string.Concat(match.Groups["Name"].Captures.Cast<Capture>().Select(x => x.Value));
        }

        private string GetDefinitionName(IEnumerable<string> definitionLines)
        {
            foreach (var line in definitionLines)
            {
                var match = _definitionNameRegex.Value.Matches(line).Cast<Match>().SingleOrDefault();

                if (match == null)
                {
                    continue;
                }

                return string.Concat(match.Groups["Name"].Captures.Cast<Capture>().Select(x => x.Value));
            }
            return null;
        }

        private bool WarnIfDefinitionMismatch(string name, string definitionName)
        {
            if (name != definitionName)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Definition mismatch. Expected: {name} | Found: {definitionName ?? "?"}");
                Console.ResetColor();
                return true;
            }
            return false;
        }

        public string GetIndexFromView(string objectName)
        {
            List<DbParameter> parameters = new List<DbParameter>();
            string result = string.Empty;

            try
            {
                parameters.Add(GetParameter("@ObjectName", objectName));

                using (DbDataReader dr = ExecuteDataReader(_generalInfo.GetIndexFromView, parameters, CommandType.Text))
                {
                    while (dr.Read())
                    {
                        if (!dr["definition"].ToString().Trim().Equals(string.Empty))
                            result += string.Format("{0}{1}{2}{3}", Environment.NewLine, CREATE_INDEX, Environment.NewLine, dr["definition"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
            return result;
        }

        public List<string> SetObject(ObjectType objectType = ObjectType.All, string objectName = "", string schemaName = "")
        {
            List<string> objectUpdated = new List<string>();
            List<DTO.DbObjectDefinition> objectDefinitions;

            try
            {
                //Get objects from Database
                objectDefinitions = GetObject(objectType, objectName, schemaName);

                //Set up filter for files
                if (schemaName.Equals(string.Empty))
                    schemaName = "*";

                if (objectName.Equals(string.Empty))
                    objectName = "*";

                //Get a list of objects to compare
                string[] files = File.GetFiles(_generalInfo.ScriptFolder, _generalInfo.FileExtension.Contains('.')
                                                        ? _generalInfo.FileExtension : string.Format("{0}.{1}.{2}", schemaName, objectName, _generalInfo.FileExtension));

                //Compare objects
                foreach (string fileName in files)
                {
                    string objectName2 = fileName.Split('\\').Last();

                    objectUpdated.AddRange(
                                    Update(objectType, fileName, objectDefinitions.Where(f => f.Schema.Trim().Equals(objectName2.Substring(0, objectName2.IndexOf('.')).Trim(), StringComparison.OrdinalIgnoreCase)
                                                                                   && f.Name.Trim().Equals(objectName2.Substring(objectName2.IndexOf('.') + 1,
                                                                                   objectName2.LastIndexOf('.') - objectName2.IndexOf('.') - 1).Trim(), StringComparison.OrdinalIgnoreCase)
                                                                                   ).FirstOrDefault())
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }

            return objectUpdated;
        }

        public List<DTO.DbObjectDefinition> SaveObject(List<DTO.DbObjectDefinition> dbObjectDefinitions)
        {
            List<DTO.DbObjectDefinition> result = new List<DTO.DbObjectDefinition>();

            foreach (var item in dbObjectDefinitions)
            {
                string fileName = GetFullFileName(item.Schema, item.Name);
                string objectName = fileName.Split('\\').Last();

                List<string> objectDefinitionLocal = System.IO.File.Exists(fileName) ? System.IO.File.ReadLines(fileName, Util.GetEncoding(fileName)).ToList() : new List<string>();
                string objectTypeLocal = GetObjectTypeFromDefinition(string.Join(Environment.NewLine, objectDefinitionLocal));
                List<string> objectDefinitionRemote = item.Definition.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();

                //Update when necessary
                if (!AreObjectsEquals(objectName, objectDefinitionLocal, objectDefinitionRemote, objectTypeLocal))
                {
                    CompareNameToDefinitionName($"{item.Schema}.{item.Name}", item.Definition);
                    Console.WriteLine(string.Format("Saving object {0}.", objectName));
                    Console.WriteLine("");
                    File.WriteFile(GetFullFileName(item.Schema, item.Name), item.Definition);
                    result.Add(item);
                }
            }

            return result;
        }

        private List<string> Update(ObjectType objectType, string fileName, DTO.DbObjectDefinition dbObjectDefinition)
        {
            List<string> objectUpdated = new List<string>();
            List<string> objectDefinitionLocal = System.IO.File.ReadLines(fileName, Util.GetEncoding(fileName)).ToList();
            string objectTypeLocal = GetObjectTypeFromDefinition(string.Join(Environment.NewLine, objectDefinitionLocal));
            List<string> objectDefinitionRemote = dbObjectDefinition != null ? dbObjectDefinition.Definition.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList() : new List<string>();
            string objectName = fileName.Split('\\').Last();

            try
            {
                //Check if object type should be updated
                var objectTypeList = objectType.GetValue();
                if (objectType.Equals(ObjectType.All) || objectTypeList.Contains(objectTypeLocal))
                {
                    //Clean up objects before compare
                    objectDefinitionLocal = CleanObjectDefinition(objectDefinitionLocal);
                    objectDefinitionRemote = CleanObjectDefinition(objectDefinitionRemote);

                    //Update when necessary
                    if (dbObjectDefinition == null || !AreObjectsEquals(objectName, objectDefinitionLocal, objectDefinitionRemote, objectTypeLocal))
                    {
                        CompareNameToDefinitionName(System.IO.Path.GetFileNameWithoutExtension(fileName), objectDefinitionLocal);
                        string[] queries = string.Join(Environment.NewLine, objectDefinitionLocal).Split(new string[] { CREATE_INDEX }, StringSplitOptions.None);
                        KeyValuePair<string, string> keyValuePair = GetReplaceString(objectTypeLocal);

                        for (int i = 0; i < queries.Length; i++)
                        {
                            //Remove Encryptation if exists
                            queries[i] = Regex.Replace(queries[i], "WITH ENCRYPTION*([^\\s]+)", string.Empty, RegexOptions.IgnoreCase);

                            Console.WriteLine(string.Format("Updating object {0} query {1} of {2}.", objectName, i + 1, queries.Length));
                            Console.WriteLine("");
                            if (ExecuteNonQuery(dbObjectDefinition != null ? Regex.Replace(queries[i], keyValuePair.Key, keyValuePair.Value, RegexOptions.IgnoreCase) : queries[i], commandType: CommandType.Text) != 0)
                            {
                                if (i == queries.Length - 1) //Wait until execute the last instruction 
                                    objectUpdated.Add(objectName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Filename: {fileName }");
                throw;
            }

            return objectUpdated;
        }

        private bool AreObjectsEquals(string objectName, List<string> objectDefinitionLocal, List<string> objectDefinitionRemote, string objectType)
        {
            int count;
            bool result = true;

            try
            {
                count = objectDefinitionLocal.Count;

                if (count != objectDefinitionRemote.Count)
                {
                    Console.WriteLine(string.Format("Different number of lines on {0}. Current: {1} and expected: {2}", objectName, count, objectDefinitionRemote.Count));
                    result = false;
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (string.Compare(objectDefinitionLocal[i].Trim(), objectDefinitionRemote[i].Trim()) != 0)
                        {
                            Console.WriteLine(string.Format("Difference on line {0} object {1}", i, objectName));
                            Console.WriteLine(string.Format("Local object: {0}", objectDefinitionLocal[i].Trim()));
                            Console.WriteLine(string.Format("Remote object: {0}", objectDefinitionRemote[i].Trim()));
                            result = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }

            return result;
        }

        private KeyValuePair<string, string> GetReplaceString(string objectType)
        {
            if (ObjectType.Functions.GetValue().Contains(objectType))
                return new KeyValuePair<string, string>("CREATE FUNCTION", "ALTER FUNCTION");
            else if (ObjectType.Procedures.GetValue().Contains(objectType))
                return new KeyValuePair<string, string>("CREATE PROC*([^\\s]+)", "ALTER PROCEDURE");
            else if (ObjectType.Triggers.GetValue().Contains(objectType))
                return new KeyValuePair<string, string>("CREATE TRIGGER", "ALTER TRIGGER");
            else if (ObjectType.Views.GetValue().Contains(objectType))
                return new KeyValuePair<string, string>("CREATE VIEW", "ALTER VIEW");
            else
                return new KeyValuePair<string, string>();
        }

        private string GetObjectTypeFromDefinition(string definition)
        {
            if (definition.ToUpper().Contains("CREATE FUNCTION"))
                return ObjectType.Functions.GetValue().First();
            else if (definition.ToUpper().Contains("CREATE AGGREGATE"))
                return ObjectType.AggregateFunctions.GetValue().First();
            else if (Regex.IsMatch(definition, "CREATE PROC*([^\\s]+)", RegexOptions.IgnoreCase))
                return ObjectType.Procedures.GetValue().First();
            else if (definition.ToUpper().Contains("CREATE TRIGGER"))
                return ObjectType.Triggers.GetValue().First();
            else if (definition.ToUpper().Contains("CREATE VIEW"))
                return ObjectType.Views.GetValue().First();
            else
                return string.Empty;
        }

        private List<string> CleanObjectDefinition(List<string> objectDefinition)
        {
            try
            {
                int count = 0;
                while (count < objectDefinition.Count)
                {
                    if (objectDefinition[count].Trim().Equals(string.Empty)
                        || objectDefinition[count].Trim().ToUpper().Equals("GO")
                        || objectDefinition[count].Trim().ToUpper().Contains("SET QUOTED_IDENTIFIER")
                        || objectDefinition[count].Trim().ToUpper().Contains("SET ANSI_NULLS"))
                        objectDefinition.RemoveAt(count);
                    else
                        break;
                }

                count = objectDefinition.Count - 1;
                while (count > 0)
                {
                    if (objectDefinition[count].Trim().ToUpper().Equals("GO"))
                    {
                        objectDefinition.RemoveAt(count);
                        count--;
                    }
                    else
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }

            return objectDefinition;
        }

        private string GetFullFileName(string objectSchema, string objectName)
        {
            if (!System.IO.Directory.Exists(_generalInfo.ScriptFolder))
                System.IO.Directory.CreateDirectory(_generalInfo.ScriptFolder);

            return string.Format("{0}{1}.{2}.{3}", _generalInfo.ScriptFolder, objectSchema, objectName, _generalInfo.FileExtension);
        }
    }
}
