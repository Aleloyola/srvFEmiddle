using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using comBusinessBE.BI;
using System.Threading;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
namespace srvFEmiddle.BO.Tools
{
    public class ooSftpBO
    {
        private string host = null;
        private string user = null;
        private string pass = null;
        private int port = 22;
        private SftpClient sftp = null;
        private ILog oLog { get; set; }
        
        /* Construct Object */
        public ooSftpBO(string hostIP, string port, string userName, string password, ILog log)
        {
            this.host = hostIP;
            this.user = userName;
            this.pass = password;
            this.port = int.Parse(port);
            this.oLog = log;
        }

        public List<string> directoryListSimple(string directory)
        {
            this.sftp = new SftpClient(this.host, this.port, this.user, this.pass);
            sftp.Connect();
            List<SftpFile> files = sftp.ListDirectory(directory).ToList();
            List<string> resp = new List<string>();

            foreach (SftpFile file in files)
            {
                if(!file.Name.Equals(".") && !file.Name.Equals(".."))   resp.Add(file.Name);
            }
            return resp;
        }

        public bool bCheckFileDownload(string localFile)
        {
            bool bResult = false;
            if (File.Exists(localFile)) oLog.LogInfo("Existe el archivo");
            else oLog.LogInfo("No existe el archivo");

            int elapsed = 0;
            while ((!bResult) && (elapsed < 5000))
            {
                Thread.Sleep(1000);
                elapsed += 1000;
                if (File.Exists(localFile)) bResult = true;
            }

            if (File.Exists(localFile)) oLog.LogInfo("Existe el archivo");
            else oLog.LogInfo("No existe el archivo");

            return bResult;
        }

        public bool download(string remoteFile, string localFile)
        {
            bool bResult = false;
            try
            {
                this.sftp = new SftpClient(this.host, this.port, this.user, this.pass);
                sftp.Connect();
                var file = File.OpenWrite(localFile);
                this.sftp.DownloadFile(remoteFile, file);
                file.Close();
                bResult = true;
            }
            catch (Exception ex)
            {
                oLog.LogError(ex.Message);
            }
            return bResult;
        }

        /* Upload File */
        public bool upload(string sDirectory, string sRemoteFile, string sLocalFile)
        {
            bool bResult = true;
            try
            {
                this.sftp = new SftpClient(this.host, this.port, this.user, this.pass);
                sftp.Connect();
                var file = File.OpenRead(sLocalFile);
                if (sDirectory.Length > 0) sftp.ChangeDirectory(sDirectory);
                sftp.UploadFile(file, sRemoteFile);
                file.Close();
                bResult = true;
            }
            catch (Exception ex)
            {
                oLog.LogError(ex.Message);
            }
            return bResult;
        }

        /* Rename File */
        public bool rename(string oldFileNameAndPath, string newFileName)
        {
            bool bResult = false;
            try
            {
                sftp.RenameFile(oldFileNameAndPath, newFileName);
                bResult = true;
            }
            catch (Exception ex)
            {
                this.oLog.LogError("Current: " + oldFileNameAndPath + " New: " + newFileName + "Exception:" + ex.Message);
            }
            return bResult;
        } 
        public void close()
        {
            sftp.Disconnect();
            sftp.Dispose();
            sftp = null;
        }


    }
}
