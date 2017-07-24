using System.IO;
using System.Net;
using System;
using System.Threading;
using comBusinessBE.BI;
using comBusinessBE.BE;

namespace srvFEmiddle.BO.Tools
{
    public class ooFtpBO
    {
        private string host = null;
        private string user = null;
        private string pass = null;
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;
        private int bufferSize = 2048;
        private ILog oLog { get; set; }
        
        /* Construct Object */
        public ooFtpBO(string hostIP, string userName, string password, ILog log)
        {
            host = hostIP;
            user = userName;
            pass = password;
            oLog = log;
        }

        private string cServer(string cStringPath)
        {
            return cStringPath.Substring(6);
        }
        /// <summary>
        /// Descarga el archivo utilizando la linea de comando.
        /// </summary>
        /// <param name="oInfoDoc">Información del documento</param>
        /// <param name="cFileName">Nombre del archivo a descargar</param>
        /// <returns></returns>
        public bool downloadShell(ooInfoDocumentoBE oInfoDoc, string cFileName)
        {
            bool bResp = false;
            String ftpCmnds = "open " + this.cServer(oInfoDoc.sInPath) + "\r\n" + oInfoDoc.sInUsuarioFTP + "\r\n" + oInfoDoc.sInPasswordFTP + "\r\ncd " + oInfoDoc.sInPathWork + "\r\nget " + cFileName + "\r\nclose\r\nquit";
            //oLog.LogInfo(ftpCmnds);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "tmp.txt")))
            {
                sw.Write(ftpCmnds);
            }

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            
            p.StartInfo = info;
            p.Start();
            //p.WaitForExit();  
            //oLog.LogInfo("se inicia proceso");
            using (System.IO.StreamWriter sw = p.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    p.WaitForExit(1000);
                        //Console.WriteLine("Forcing Download from " + folder.server + folder.path + " of " + file + "\n"); log += "\r\n\r\n\t\t- Forcing Download from " + folder.server + folder.path + file + "\tto\t" + basePath + "\\" + Util.getDateFormated(reverseDate) + "\\" + parentFolder + "\\" + folder.server + "\\" + file;
                        //oLog.LogInfo("Cambia de directorio a:" + System.AppDomain.CurrentDomain.BaseDirectory);
                        sw.WriteLine("cd " + System.AppDomain.CurrentDomain.BaseDirectory);
                        //oLog.LogInfo("Ejecuta archivo tmp.txt");
                        sw.WriteLine("ftp -s:tmp.txt");
                        sw.WriteLine("del tmp.txt");
                        p.Close();
                    
                }
            }
            
            bResp = true;
            return bResp;
        }

        public bool bCheckFileDownload(string localFile)
        {
            bool bResp = false;
            if (File.Exists(localFile)) oLog.LogInfo("Existe el archivo");
            else oLog.LogInfo("No existe el archivo");
            
            int elapsed = 0;
            while ((!bResp) && (elapsed < 5000))
            {
                Thread.Sleep(1000);
                elapsed += 1000;
                if (File.Exists(localFile)) bResp = true;
            }

            if (File.Exists(localFile)) oLog.LogInfo("Existe el archivo");
            else oLog.LogInfo("No existe el archivo");
                
            return bResp;
        }
        public bool renameShell(ooInfoDocumentoBE oInfoDoc, string cOriginalName, string cNewName)
        {
            bool bResp = false;
            String ftpCmnds = "open " + this.cServer(oInfoDoc.sInPath) + "\r\n" + oInfoDoc.sInUsuarioFTP + "\r\n" + oInfoDoc.sInPasswordFTP + "\r\ncd " + oInfoDoc.sInPathWork + "\r\nrename " + cOriginalName + " " + cNewName + "\r\n DELE " + cOriginalName.Substring(0,cOriginalName.IndexOf('.')) + "\r\nclose\r\nquit";
            //oLog.LogInfo(ftpCmnds);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "tmp2.txt")))
            {
                sw.Write(ftpCmnds);
            }

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;

            p.StartInfo = info;
            p.Start();
            //p.WaitForExit();  
            //oLog.LogInfo("se inicia proceso");
            using (System.IO.StreamWriter sw = p.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    p.WaitForExit(1000);
                        //Console.WriteLine("Forcing Download from " + folder.server + folder.path + " of " + file + "\n"); log += "\r\n\r\n\t\t- Forcing Download from " + folder.server + folder.path + file + "\tto\t" + basePath + "\\" + Util.getDateFormated(reverseDate) + "\\" + parentFolder + "\\" + folder.server + "\\" + file;
                        //oLog.LogInfo("Cambia de directorio a:" + System.AppDomain.CurrentDomain.BaseDirectory);
                        sw.WriteLine("cd " + System.AppDomain.CurrentDomain.BaseDirectory);
                        //oLog.LogInfo("Ejecuta archivo tmp.txt");
                        sw.WriteLine("ftp -s:tmp2.txt");
                        sw.WriteLine("del tmp2.txt");
                        p.Close();
                    
                }
            }
            bResp = true;
            return bResp;
        }
        /* Download File */
        public bool download(string remoteFile, string localFile)
        {
            bool bResult = false;
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Get the FTP Server's Response Stream */
                ftpStream = ftpResponse.GetResponseStream();
                /* Open a File Stream to Write the Downloaded File */
                FileStream localFileStream = new FileStream(localFile, FileMode.Create);
                /* Buffer for the Downloaded Data */
                byte[] byteBuffer = new byte[bufferSize];
                int bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                
                /* Download the File by Writing the Buffered Data Until the Transfer is Complete */
                try
                {
                    while (bytesRead > 0)
                    {
                        localFileStream.Write(byteBuffer, 0, bytesRead);
                        bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex)
                {
                    oLog.LogError(ex.Message);
                }
                /* Resource Cleanup */
                localFileStream.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                bResult = true;
            }
            catch (Exception ex)
            {
                oLog.LogError(ex.Message);
            }
            return bResult;
        }

        public void downloadTest(string remoteFile, string localFile)
        {
            try
            {
                /* Create an FTP Request */
                Uri u = new Uri(remoteFile);
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(u);

                //ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                oLog.LogInfo("Se iniciará conexión a:" + ftpRequest.RequestUri);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Get the FTP Server's Response Stream */
                ftpStream = ftpResponse.GetResponseStream();
                /* Open a File Stream to Write the Downloaded File */
                FileStream localFileStream = new FileStream(localFile, FileMode.Create);
                /* Buffer for the Downloaded Data */
                byte[] byteBuffer = new byte[bufferSize];
                int bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);

                /* Download the File by Writing the Buffered Data Until the Transfer is Complete */
                try
                {
                    while (bytesRead > 0)
                    {
                        oLog.LogInfo("El archivo tiene contenido");
                        localFileStream.Write(byteBuffer, 0, bytesRead);
                        bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex)
                {
                    oLog.LogError(ex.Message);
                }
                /* Resource Cleanup */
                localFileStream.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                oLog.LogError(ex.Message);
            }
            oLog.LogInfo("Se termina funcion Download");
            return;
        }

        public bool uploadShell(ooInfoDocumentoBE oInfoDoc, string cFileName)
        { 
            bool bResp = false;
            String ftpCmnds = "open " + this.cServer(oInfoDoc.sOutPath) + "\r\n" + oInfoDoc.sOutUsuarioFTP + "\r\n" + oInfoDoc.sOutPasswordFTP + "\r\nascii\r\nput " + cFileName + "\r\nclose\r\nquit";
            oLog.LogInfo(ftpCmnds);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "tmp2.txt")))
            {
                sw.Write(ftpCmnds);
            }

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;

            p.StartInfo = info;
            p.Start();
            //oLog.LogInfo("se inicia proceso");
            using (System.IO.StreamWriter sw = p.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    //Console.WriteLine("Forcing Download from " + folder.server + folder.path + " of " + file + "\n"); log += "\r\n\r\n\t\t- Forcing Download from " + folder.server + folder.path + file + "\tto\t" + basePath + "\\" + Util.getDateFormated(reverseDate) + "\\" + parentFolder + "\\" + folder.server + "\\" + file;
                    //oLog.LogInfo("Cambia de directorio a:" + System.AppDomain.CurrentDomain.BaseDirectory);
                    sw.WriteLine("cd " + System.AppDomain.CurrentDomain.BaseDirectory);
                    //oLog.LogInfo("Ejecuta archivo tmp.txt");
                    sw.WriteLine("ftp -s:tmp2.txt");
                    sw.WriteLine("del tmp2.txt");
                    p.Close();
                }
            }
            bResp = true;
            return bResp;

        }

        public bool upload2(string remoteFile,string localFilePath)
        {
            string remoteFilePath=this.host + "/" + remoteFile;
            bool b = true;

            if (File.Exists(localFilePath)) oLog.LogInfo("Existe el archivo");
            else
                oLog.LogInfo("No existe el archivo");

            while (!File.Exists(localFilePath))
            { }

            if (File.Exists(localFilePath)) oLog.LogInfo("Existe el archivo");
            else
                oLog.LogInfo("No existe el archivo");
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(this.user, this.pass);
                    client.UploadFile(remoteFilePath, "STOR", localFilePath);
                }
            }
            catch (Exception ex)
            {
                this.oLog.LogError(ex.Message);
                b = false;
            }
            return b;
        }

        /* Upload File */
        public bool upload(string remoteFile, string localFile)
        {
            bool bState = true;
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpRequest.GetRequestStream();
                /* Open a File Stream to Read the File for Upload */
                FileStream localFileStream = new FileStream(localFile, FileMode.Open);
                /* Buffer for the Downloaded Data */
                byte[] byteBuffer = new byte[bufferSize];
                int bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
                try
                {
                    while (bytesSent != 0)
                    {
                        ftpStream.Write(byteBuffer, 0, bytesSent);
                        bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex)
                {
                    bState = false;
                    oLog.LogError(ex);
                }
                /* Resource Cleanup */
                localFileStream.Close();
                ftpStream.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                bState = false;
                oLog.LogError(ex);
            }
            return bState;
        }


        /* Delete File */
        public void delete(string deleteFile)
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + deleteFile);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile; 
                /* Establish Return Communication with the FTP Server */ 
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse(); 
                /* Resource Cleanup */ 
                ftpResponse.Close(); 
                ftpRequest = null;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString()); //ToDo: cambiar a log de la app 
            } 
            return;
        } 
        
        public void FtpRename( string source, string destination ) {
        if ( source == destination )
            return;
        try
        {
            Uri uriSource = new Uri(this.host + "/" + source, UriKind.Absolute);
            Uri uriDestination = new Uri(this.host + "/" + destination, UriKind.Absolute);


            Uri targetUriRelative = uriSource.MakeRelativeUri(uriDestination);


            //perform rename
            FtpWebRequest ftp = (FtpWebRequest)WebRequest.Create(uriSource.AbsoluteUri);
            ftp.Credentials = new NetworkCredential(user, pass);
            ftp.UseBinary = true;
            ftp.UsePassive = false;
            ftp.KeepAlive = false;
            ftp.Method = WebRequestMethods.Ftp.Rename;
            ftp.RenameTo = Uri.UnescapeDataString(targetUriRelative.OriginalString);

            FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();
        }
        catch (Exception ex)
        {
            this.oLog.LogError(ex.Message);
        }
        return; 

    }
        /* Rename File */ 
        public void rename(string currentFileNameAndPath, string newFileName) 
        { 
            try 
            { 
                /* Create an FTP Request */ 
                ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + currentFileNameAndPath); 
                /* Log in to the FTP Server with the User Name and Password Provided */ 
                ftpRequest.Credentials = new NetworkCredential(user, pass); 
                /* When in doubt, use these options */ 
                ftpRequest.UseBinary = true; 
                ftpRequest.UsePassive = false; 
                ftpRequest.KeepAlive = true; 
                /* Specify the Type of FTP Request */ 
                ftpRequest.Method = WebRequestMethods.Ftp.Rename; 
                /* Rename the File */
                ftpRequest.RenameTo = newFileName; 
                /* Establish Return Communication with the FTP Server */ 
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse(); 
                /* Resource Cleanup */ 
                ftpResponse.Close(); 
                ftpRequest = null; 
            } 
            catch (Exception ex) 
            { 
               this.oLog.LogError("Current: "+currentFileNameAndPath+" New: "+ newFileName+ "Exception:" +ex.Message);
            } 
            return; 
        } 
        
        /* Create a New Directory on the FTP Server */ 
        public void createDirectory(string newDirectory) 
        { 
            try 
            { 
                /* Create an FTP Request */ 
                ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + newDirectory); 
                /* Log in to the FTP Server with the User Name and Password Provided */ 
                ftpRequest.Credentials = new NetworkCredential(user, pass); 
                /* When in doubt, use these options */ 
                ftpRequest.UseBinary = true; 
                ftpRequest.UsePassive = true; 
                ftpRequest.KeepAlive = true; 
                /* Specify the Type of FTP Request */ 
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory; 
                /* Establish Return Communication with the FTP Server */ 
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse(); 
                /* Resource Cleanup */ 
                ftpResponse.Close(); 
                ftpRequest = null; 
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.ToString()); 
            } 
            return; 
        } 
        
        /* Get the Date/Time a File was Created */ 
        public string getFileCreatedDateTime(string fileName) 
        { 
            try 
            { 
                /* Create an FTP Request */ 
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + fileName); 
                /* Log in to the FTP Server with the User Name and Password Provided */ 
                ftpRequest.Credentials = new NetworkCredential(user, pass); 
                /* When in doubt, use these options */ 
                ftpRequest.UseBinary = true; 
                ftpRequest.UsePassive = true; 
                ftpRequest.KeepAlive = true; 
                /* Specify the Type of FTP Request */ 
                ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp; 
                /* Establish Return Communication with the FTP Server */ 
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse(); 
                /* Establish Return Communication with the FTP Server */ 
                ftpStream = ftpResponse.GetResponseStream(); 
                /* Get the FTP Server's Response Stream */ 
                StreamReader ftpReader = new StreamReader(ftpStream); 
                /* Store the Raw Response */ 
                string fileInfo = null; 
                /* Read the Full Response Stream */ 
                try 
                { 
                    fileInfo = ftpReader.ReadToEnd(); 
                } 
                catch (Exception ex)
                { 
                    Console.WriteLine(ex.ToString()); 
                } 
                /* Resource Cleanup */ 
                ftpReader.Close(); 
                ftpStream.Close(); 
                ftpResponse.Close(); 
                ftpRequest = null; 
                /* Return File Created Date Time */ 
                return fileInfo; 
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.ToString()); 
            } 
            /* Return an Empty string Array if an Exception Occurs */ 
            return ""; 
        } 

        /* Get the Size of a File */ 
        public string getFileSize(string fileName) 
        { 
            try 
            { 
                /* Create an FTP Request */ 
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + fileName); 
                /* Log in to the FTP Server with the User Name and Password Provided */ 
                ftpRequest.Credentials = new NetworkCredential(user, pass); 
                /* When in doubt, use these options */ 
                ftpRequest.UseBinary = true; 
                ftpRequest.UsePassive = true; 
                ftpRequest.KeepAlive = true; 
                /* Specify the Type of FTP Request */ 
                ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize; 
                /* Establish Return Communication with the FTP Server */ 
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse(); 
                /* Establish Return Communication with the FTP Server */ 
                ftpStream = ftpResponse.GetResponseStream(); 
                /* Get the FTP Server's Response Stream */ 
                StreamReader ftpReader = new StreamReader(ftpStream); 
                /* Store the Raw Response */ 
                string fileInfo = null; 
                /* Read the Full Response Stream */ 
                try 
                { 
                    while (ftpReader.Peek() != -1) 
                    { 
                        fileInfo = ftpReader.ReadToEnd(); 
                    } 
                } 
                catch (Exception ex) 
                { 
                    Console.WriteLine(ex.ToString()); 
                } 
                /* Resource Cleanup */ 
                ftpReader.Close(); 
                ftpStream.Close(); 
                ftpResponse.Close(); 
                ftpRequest = null; 
                /* Return File Size */ 
                return fileInfo; 
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.ToString()); 
            } 
            /* Return an Empty string Array if an Exception Occurs */ 
            return ""; 
        } 
        
        /* List Directory Contents File/Folder Name Only */ 
        public string[] directoryListSimple(string directory) 
        { 
            try 
            { 
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + directory + "/"); 
                /* Log in to the FTP Server with the User Name and Password Provided */ 
                ftpRequest.Credentials = new NetworkCredential(user, pass); 
                /* When in doubt, use these options */ 
                ftpRequest.UseBinary = true; 
                ftpRequest.UsePassive = true; 
                ftpRequest.KeepAlive = true; 
                /* Specify the Type of FTP Request */ 
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory; 
                /* Establish Return Communication with the FTP Server */ 
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse(); 
                /* Establish Return Communication with the FTP Server */ 
                ftpStream = ftpResponse.GetResponseStream(); 
                /* Get the FTP Server's Response Stream */ 
                StreamReader ftpReader = new StreamReader(ftpStream); 
                /* Store the Raw Response */ 
                string directoryRaw = null; 
                /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */ 
                try 
                { 
                    while (ftpReader.Peek() != -1) 
                    { 
                        directoryRaw += ftpReader.ReadLine() + "|"; 
                    }
                } 
                catch (Exception ex) 
                { 
                    Console.WriteLine(ex.ToString()); 
                } 
                /* Resource Cleanup */ 
                ftpReader.Close(); 
                ftpStream.Close(); 
                ftpResponse.Close(); 
                ftpRequest = null; 
                /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */ 
                try { 
                    string[] directoryList = directoryRaw.Split("|".ToCharArray()); 
                    return directoryList; 
                } 
                catch (Exception ex) 
                { 
                    Console.WriteLine(ex.ToString()); 
                } 
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.ToString()); 
            } 
            /* Return an Empty string Array if an Exception Occurs */ 
            return new string[] { "" }; 
        } 
        
        /* List Directory Contents in Detail (Name, Size, Created, etc.) */ 
        public string[] directoryListDetailed(string directory) 
        { 
            try 
            { 
                /* Create an FTP Request */ 
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + directory); 
                /* Log in to the FTP Server with the User Name and Password Provided */ 
                ftpRequest.Credentials = new NetworkCredential(user, pass); 
                /* When in doubt, use these options */ 
                ftpRequest.UseBinary = true; 
                ftpRequest.UsePassive = true; 
                ftpRequest.KeepAlive = true; 
                /* Specify the Type of FTP Request */ 
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails; 
                /* Establish Return Communication with the FTP Server */ 
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse(); 
                /* Establish Return Communication with the FTP Server */ 
                ftpStream = ftpResponse.GetResponseStream(); 
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(ftpStream); 
                /* Store the Raw Response */ 
                string directoryRaw = null; 
                /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */ 
                try 
                { 
                    while (ftpReader.Peek() != -1) 
                    { 
                        directoryRaw += ftpReader.ReadLine() + "|"; 
                    } 
                } 
                catch (Exception ex) 
                { 
                    Console.WriteLine(ex.ToString()); 
                } 
                /* Resource Cleanup */ 
                ftpReader.Close(); 
                ftpStream.Close(); 
                ftpResponse.Close(); 
                ftpRequest = null; 
                /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */ 
                try 
                { 
                    string[] directoryList = directoryRaw.Split("|".ToCharArray()); 
                    return directoryList; 
                } 
                catch (Exception ex) 
                { 
                    Console.WriteLine(ex.ToString()); 
                } 
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.ToString());
            } 
            /* Return an Empty string Array if an Exception Occurs */ 
            return new string[] { "" }; 
        }
    }
}
