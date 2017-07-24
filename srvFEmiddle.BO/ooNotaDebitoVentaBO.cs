using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using srvFEmiddle.BO.Tools;

namespace srvFEmiddle.BO
{
    public class ooNotaDebitoVentaBO : ooOperacionBO
    {
        public override List<string> getLeerDocumentos()
        {
            return oInfoNotaDebitoVenta.bIsSFTP ? this.SFTPleerDocumentos(oInfoNotaDebitoVenta) : this.leerDocumentos(oInfoNotaDebitoVenta);
        }

        public override int getMoverDocumentos(List<string> lstFiles)
        {
            return oInfoNotaDebitoVenta.bIsSFTP ? this.SFTPmoverDocumentos (oInfoNotaDebitoVenta, lstFiles) : this.moverDocumentos(oInfoNotaDebitoVenta, lstFiles);
        }
    }
}
