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
            return this.leerDocumentos(oInfoNotaCreditoVenta);
        }

        public override int getMoverDocumentos(List<string> lstFiles)
        {
            return this.moverDocumentos(oInfoNotaCreditoVenta, lstFiles);
        }
    }
}
