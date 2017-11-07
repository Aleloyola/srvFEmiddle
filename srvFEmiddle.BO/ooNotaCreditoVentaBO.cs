using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using srvFEmiddle.BO.Tools;

namespace srvFEmiddle.BO
{
    public class ooNotaCreditoVentaBO : ooOperacionBO
    {
        public override List<string> getLeerDocumentos()
        {
            return oInfoNotaCreditoVenta.bIsSFTP ? this.SFTPleerDocumentos(oInfoNotaCreditoVenta) : this.leerDocumentos(oInfoNotaCreditoVenta);
        }

        public override int getMoverDocumentos(List<string> lstFiles)
        {
            if (!oInfoNotaCreditoVenta.bIsNoElectronic)
            {
                return oInfoNotaCreditoVenta.bIsSFTP ? this.SFTPmoverDocumentos(oInfoNotaCreditoVenta, lstFiles) : this.moverDocumentos(oInfoNotaCreditoVenta, lstFiles);
            }
            else
                return this.moverDocumentosNoElectronicos(oInfoNotaCreditoVenta, lstFiles);
        }

    }
}
