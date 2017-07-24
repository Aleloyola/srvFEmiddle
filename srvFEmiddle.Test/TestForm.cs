using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
 

namespace srvFEmiddle.Test
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
            //srvFEmiddle.BD.ooConnectionBD oConnection = new srvFEmiddle.BD.ooConnectionBD();
            //this.textBox1.Text = oConnection.select();
            String Host = "198.46.225.184";
            int Port = 22;
            String RemoteFileName = "test3.txt";
            String LocalDestinationFilename = "test.txt";
            String Username = "usersftp";
            String Password = "usersftppassword";
            Encoding encod = Encoding.UTF8;
            List<SftpFile> archivos;
            var sftp = new SftpClient(Host, Port, Username, Password);
            
            sftp.Connect();
            /*var file = File.OpenWrite(LocalDestinationFilename);
            archivos = sftp.ListDirectory("/test_readwrite").ToList();
            this.textBox2.Text = archivos.Count.ToString();*/
            //sftp.DownloadFile(RemoteFileName, file);

            //var archi = new FileStream(RemoteFileName, FileMode.Open);
            sftp.ChangeDirectory("test_readwrite/algo/prueba");
            //sftp.UploadFile(archi, "test2.txt");
            //var algo = sftp.ReadAllLines("test_readwrite");
            sftp.Disconnect();
           
        }
    }
}
