using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace comBusinessBE.BE
{
    public class ooInfoDocumentoBE
    {
        public string sInPath { get; set; }
        public string sInUsuarioFTP { get; set; }
        public string sInPasswordFTP { get; set; }
        public string sInPathWork { get; set; }
        public string sInPathProcessed { get; set; }
        public string sInPathError { get; set; }
        public string sOutPath { get; set; }
        public string sOutSFTPpath { get; set; }
        public string sOutUsuarioFTP { get; set; }
        public string sOutPasswordFTP { get; set; }
        public bool bIsStandard { get; set; }
        public bool bIsSFTP { get; set; }
        public string sInPort { get; set; }
        public string sOutPort { get; set; }
    }
}
