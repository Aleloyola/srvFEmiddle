﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using srvFEmiddle.BO.Tools;

namespace srvFEmiddle.BO
{
    public class ooFacturaVentaBO : ooOperacionBO
    {
        public override List<string> getLeerDocumentos()
        {
            return oInfoFacturaVenta.bIsSFTP ? this.SFTPleerDocumentos(oInfoFacturaVenta) : this.leerDocumentos(oInfoFacturaVenta);
        }

        public override int getMoverDocumentos(List<string> lstFiles)
        {
            if (!oInfoFacturaVenta.bIsNoElectronic)
            {
                return oInfoFacturaVenta.bIsSFTP ? this.SFTPmoverDocumentos(oInfoFacturaVenta, lstFiles) : this.moverDocumentos(oInfoFacturaVenta, lstFiles);
            }
            else
                return this.moverDocumentosNoElectronicos(oInfoFacturaVenta, lstFiles);                                                                            
        }
    }
}
                                