using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using srvFEmiddle.BO.Tools;

namespace srvFEmiddle.BO
{
    public class ooFacturaVentaBO : ooOperacionBO
    {
        /*public override List<string> LeerDocumentos()
        {
            List<string> lstFiles;
            ooFileBO oFiles = new ooFileBO();
            lstFiles = oFiles.getFiles(oInfoFacturaVenta.sPathWork).ToList<string>();

            this.oLog.LogInfo(String.Format("Inicio lectura de operaciones de tipo {0} en estado sin procesar.", lstFiles.Count));
            foreach (string item in lstFiles)
            {
                try
                {
                    ooFtpBO oFtp = new ooFtpBO(oInfoFacturaVenta.sPathFTP, oInfoFacturaVenta.sUsuarioFTP, oInfoFacturaVenta.sPasswordFTP,oLog);
                    bool bState = oFtp.upload(oFiles.getFileName(item), item);
                    if (bState)
                        oFiles.moveFile(item, oInfoFacturaVenta.sPathProcessed);
                    else
                        oFiles.moveFile(item, oInfoFacturaVenta.sPathError);
                }
                catch (Exception ex)
                {
                    oFiles.moveFile(item, oInfoFacturaVenta.sPathError);
                    this.oLog.LogError(String.Format("Se ha producido un error al traspasar el archivo {0}, con la siguiente descripcion: {1}.", item, ex.Message));
                }
            }
            return lstFiles;
        }*/
        public override List<string> getLeerDocumentos()
        {
            return this.leerDocumentos(oInfoFacturaVenta);
        }

        public override int getMoverDocumentos(List<string> lstFiles)
        {
            return this.moverDocumentos(oInfoFacturaVenta, lstFiles);
        }
    }
}
