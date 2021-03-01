using System;
using System.IO;

namespace MTFO.Utilities
{
    // Not sure if this should go into Extensions or not.
    public static class FileUtil
    {
        public static bool TryReadAllText(string path, out string text)
        {
            text = null;

            try
            {
                text = File.ReadAllText(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
