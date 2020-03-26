using NLog;
using System;
using System.IO;
using System.Text;

namespace Swift.Database.Helper.Generic
{
    public static class File
    {
        private static readonly Logger _logger;
        static File()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }
        public static void WriteFile(string fullFileName, string fileData, bool append = false)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(fullFileName, append, new UTF8Encoding(true)))
                {
                    if (append)
                        sw.WriteLine(fileData);
                    else
                        sw.Write(fileData);

                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        public static bool DeleteFile(string fullFileName)
        {
            bool result = false;
            try
            {
                if (System.IO.File.Exists(fullFileName))
                    System.IO.File.Delete(fullFileName);

                result = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }

            return result;
        }

        public static bool DeleteManyFile(string pathName, string fileExtension = "*")
        {
            bool result = false;
            try
            {
                string[] files = Directory.GetFiles(pathName, fileExtension);

                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }

                result = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }

            return result;
        }

        public static string[] GetFiles(string pathName, string fileExtension = "*")
        {
            string[] files;
            try
            {
                files = Directory.GetFiles(pathName, fileExtension);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }

            return files;
        }
    }
}
