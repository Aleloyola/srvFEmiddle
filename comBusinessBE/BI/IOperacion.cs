using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using comBusinessBE.BE;

namespace comBusinessBE.BI
{
    public interface IOperacion
    {
        ILog oLog { get; set; }
        int lTraspasar();
        List<string> leerDocumentos(ooInfoDocumentoBE oInfoDocumentoBE);
        List<string> getLeerDocumentos();
        int getMoverDocumentos(List<string> lstFiles);
        int moverDocumentos(ooInfoDocumentoBE oInfoDoc, List<string> lstFiles);
        
        List<string> SFTPleerDocumentos(ooInfoDocumentoBE oInfoDocumentoBE);
        int SFTPmoverDocumentos(ooInfoDocumentoBE oInfoDoc, List<string> lstFiles);

        int moverDocumentosNoElectronicos(ooInfoDocumentoBE oInfoDoc, List<string> lstFiles);
    }
}
