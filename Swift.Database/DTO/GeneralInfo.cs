using System.IO;

namespace Swift.Database.DTO
{
    public class GeneralInfo
    {
        public string ConnectionString { get; set; }

        private string _scriptFolder;

        public string ScriptFolder
        {
            get { return _scriptFolder; }
            set
            {
                if (value.EndsWith(@"\"))
                    _scriptFolder = value;
                else
                    _scriptFolder = string.Format(@"{0}\", value);
            }
        }

        private string _baseDirectory;
        public string BaseDirectory
        {
            get { return _baseDirectory; }
            set
            {
                if (value.EndsWith(@"\"))
                    _baseDirectory = value;
                else
                    _baseDirectory = string.Format(@"{0}\", value);
            }
        }

        public string FileExtension { get; set; }

        public bool GenerateReport { get; set; }

        //public string ReportFileName { get; set; }

        private string _objectDefinition { get; set; }

        public string GetObjectDefinition
        {
            get
            {
                if (_objectDefinition == null || string.Equals(_objectDefinition, string.Empty, System.StringComparison.OrdinalIgnoreCase))
                    _objectDefinition = File.ReadAllText(string.Format(@"{0}Scripts\{1}", BaseDirectory, "spA_GetObjectDefinition.sql"));

                return _objectDefinition;
            }
        }

        private string _indexFromView { get; set; }
        public string GetIndexFromView
        {
            get
            {
                if (_indexFromView == null || string.Equals(_indexFromView, string.Empty, System.StringComparison.OrdinalIgnoreCase))
                    _indexFromView = File.ReadAllText(string.Format(@"{0}Scripts\{1}", BaseDirectory, "spA_GetIndexFromView.sql"));

                return _indexFromView;
            }
        }
    }
}
