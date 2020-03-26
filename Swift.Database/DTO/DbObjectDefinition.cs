using System.Data.Common;

namespace Swift.Database.DTO
{
    public class DbObjectDefinition
    {
        public string ObjectID { get; set; }
        public string Type { get; set; }
        public string TypeDescription { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public string Definition { get; set; }
        public DbObjectDefinition() { }

        public DbObjectDefinition(DbDataReader dbDataReader)
        {
            GetData(dbDataReader);
        }

        public void GetData(DbDataReader dbDataReader)
        {
            ObjectID = dbDataReader["object_id"].ToString();
            Type = dbDataReader["type"].ToString().Trim();
            TypeDescription = dbDataReader["type_desc"].ToString();
            Name = dbDataReader["name"].ToString();
            Schema = dbDataReader["schema"].ToString();
            Definition = dbDataReader["definition"].ToString();
        }
    }
}