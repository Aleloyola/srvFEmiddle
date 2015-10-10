using System;
using System.IO;

namespace srvFEmiddle.BO.Tools
{
    public class ooFileBO
    {
        public string getFileName(string sPathFile)
        {
            return Path.GetFileName(sPathFile);
        }

        public string[] getFiles(string sDirectory)
        {
            string[] Files = Directory.GetFiles(@sDirectory, "*.xml");
            return Files;
        }

        public void moveFile(string sSourcePath, string sDestinyPath)
        {
            sDestinyPath = Path.Combine(sDestinyPath, Path.GetFileName(sSourcePath));
            if (File.Exists(sDestinyPath)) File.Delete(sDestinyPath);
            Directory.Move(sSourcePath, sDestinyPath);
        }

        public void deleteFile(string sFileName)
        {
            File.Delete(sFileName);
        }
        
        public string sPathCombineWin(string sPath, string sFileName)
        {
            string b = Path.Combine(sPath, sFileName);
            b.Replace(@"\", @"\\");
            return b;
        }

        public string sPathCombine(string sPath, string sFileName)
        {
            return sPath + "/" + sFileName;
        }
    }
}
