using System.IO;
using System.Text;

namespace Swift.Database.Helper.Generic
{
    public static class Util
    {
        public static Encoding GetEncoding(string fullFieName)
        {
            Encoding encoding = Encoding.Default;

            using (StreamReader file = new StreamReader(fullFieName, encoding))
            {
                encoding = file.CurrentEncoding;
            }

            return encoding;
        }
    }
}
