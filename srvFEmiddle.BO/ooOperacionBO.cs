using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using comBusinessBE.BI;
using comBusinessBE.BE;
using comBusinessBE.BD;
using srvFEmiddle.BO.Tools;

namespace srvFEmiddle.BO
{
    public abstract class ooOperacionBO: IOperacion
    {
        public ILog oLog { get; set; }
        //Clases con la información de los distintos tipos de documentos tributarios
        public ooInfoDocumentoBE oInfoFacturaVenta { get; set; }
        public ooInfoDocumentoBE oInfoNotaCreditoVenta { get; set; }
        public ooInfoDocumentoBE oInfoNotaDebitoVenta { get; set; }
        public IConnectionBD oConnectionBD { get; set; }
        /// <summary>
        /// Funcion abstracta usada para invocar desde una clase hija con los parametros de ella.
        /// </summary>
        /// <returns>Lista de archivos afectados</returns>
        public abstract List<string> getLeerDocumentos();
        public abstract int getMoverDocumentos(List<string> lstFiles);
        /// <summary>
        /// Funcion padre para orquestar distintas funciones (de ser requerido)
        /// </summary>
        /// <returns></returns>
        public int lTraspasar()
        {
            return this.getMoverDocumentos(this.getLeerDocumentos());
        }
        /// <summary>
        /// Función de uso general que contiene la lógica de lectura, es invocada por cada tipo de documento con sus atributos
        /// </summary>
        /// <param name="oInfoDoc">Información del tipo de documento que utiliza la función</param>
        /// <returns>Lista de archivos afectados</returns>
        public List<string> leerDocumentos(ooInfoDocumentoBE oInfoDoc) //usado con el funcionamiento general
        {
            this.oLog.LogInfo("InPath:" + oInfoDoc.sInPath + " Usuario:" + oInfoDoc.sInUsuarioFTP + " Password:" + oInfoDoc.sInPasswordFTP);
            ooFtpBO oFtp = new ooFtpBO(oInfoDoc.sInPath, oInfoDoc.sInUsuarioFTP, oInfoDoc.sInPasswordFTP, oLog);
            List<string> lstFiles;
            List<string> lstFilesResult = new List<string>();
            this.oLog.LogInfo("Leyendo el directorio: " + oInfoDoc.sInPathWork);
            lstFiles = oFtp.directoryListSimple(oInfoDoc.sInPathWork).ToList<string>();
            foreach(string file in lstFiles){
                if (file.Contains("."))
                {
                    lstFilesResult.Add(file);
                }
            }
            this.oLog.LogInfo("Fueron encontrados " + lstFilesResult.Count + " archivos.");
            return lstFilesResult;
        }
        /// <summary>
        /// Funcion de uso general con la lógica para mover los archivos
        /// </summary>
        /// <param name="oInfoDoc"></param>
        /// <param name="lstFiles"></param>
        /// <returns></returns>
        public int moverDocumentos(ooInfoDocumentoBE oInfoDoc, List<string> lstFiles)
        {
            int lProcesados = 0;
            ooFileBO oFiles = new ooFileBO(); //For combine and remove temporary files.
            ooFtpBO oFtpIn;// = new ooFtpBO(oInfoDoc.sInPath, oInfoDoc.sInUsuarioFTP, oInfoDoc.sInPasswordFTP, oLog);//Reading server connection
            ooFtpBO oFtpOut;// = new ooFtpBO(oInfoDoc.sOutPath, oInfoDoc.sOutUsuarioFTP, oInfoDoc.sOutPasswordFTP, oLog); //Writting server connection
            this.oLog.LogInfo(String.Format("Inicio lectura de operaciones de tipo {0} en estado sin procesar.", lstFiles.Count));

            string sDocType = string.Empty; //to be used in foreach flow.
            string sDocNumber = string.Empty; //IDEM
            //oFtpOut.upload("D33_500161.D33_500161", "C:\\Program Files (x86)\\srvFEmiddle\\D33_500126.D33_500126");
            
            foreach (string item in lstFiles)
            {
                sDocType = string.Empty; 
                sDocNumber = string.Empty;
                oFtpIn = new ooFtpBO(oInfoDoc.sInPath, oInfoDoc.sInUsuarioFTP, oInfoDoc.sInPasswordFTP, oLog);

                oFtpOut = new ooFtpBO(oInfoDoc.sOutPath, oInfoDoc.sOutUsuarioFTP, oInfoDoc.sOutPasswordFTP, oLog); //Writting server connection
                if (item.Length > 0)
                {
                    oLog.LogInfo("El archivo a trabajar es:"+item);
                    
                    
                    try
                    {
                        this.oLog.LogInfo("Se está accediendo al host: "+ oInfoDoc.sInPath);
                        this.oLog.LogInfo("Se esta leyendo el archivo " + oFiles.sPathCombine(oInfoDoc.sInPathWork, item));

                        bool bDownload = oFtpIn.downloadShell(oInfoDoc, item);
                        
                        //If filename include document information, then we can continue, else need to be marked as error
                        if (bDownload && this.bDocumentInfo(out sDocType, out sDocNumber, item))
                        {
                            this.oLog.LogInfo("DocType: " + sDocType+ "DocNumber: "+sDocNumber);
                            bool bState = oFtpOut.upload(this.cNewName(item), oFiles.sPathCombineWin(System.AppDomain.CurrentDomain.BaseDirectory, item));
                            if (bState)
                            {
                                this.oLog.LogInfo("File will be moved");
                                this.bMoveFile(oFtpIn, oInfoDoc.sInPathWork, oInfoDoc.sInPathProcessed, item, oInfoDoc);
                                this.oConnectionBD.updateOK(sDocType, sDocNumber);
                                lProcesados++;
                            }
                            else
                            {
                                this.bMoveFile(oFtpIn, oInfoDoc.sInPathWork, oInfoDoc.sInPathError, item, oInfoDoc);
                                this.oConnectionBD.updateError(sDocType, sDocNumber, "Upload failed.");
                                oLog.LogInfo("Uploading file error");
                            }
                        }
                        else
                        {
                            this.oLog.LogInfo("File don't include document information");
                            this.bMoveFile(oFtpIn, oInfoDoc.sInPathWork, oInfoDoc.sInPathError, item, oInfoDoc);
                            //I can't mark in DB, because file doesn't have information to identify some register on DB.
                        }
                    }
                    catch (Exception ex)
                    {
                        this.bMoveFile(oFtpIn, oInfoDoc.sInPathWork, oInfoDoc.sInPathError, item, oInfoDoc);
                        this.oConnectionBD.updateError(sDocType, sDocNumber, ex.Message);
                        this.oLog.LogError(String.Format("Se ha producido un error al traspasar el archivo {0}, con la siguiente descripcion: {1}.", item, ex.Message));
                    }
                }
            }
            return lProcesados;
        }

        private string cNewName(string cOriginalName)
        {
            string cNewFileName = string.Empty;
            try
            {

                if (cOriginalName.IndexOf('.') > 0)
                {
                    cNewFileName = cOriginalName.Substring(1, cOriginalName.IndexOf('.')) + "txt";
                }
                
            }
            catch (Exception ex)
            {
                this.oLog.LogError(ex.Message);
            }
            cNewFileName = cNewFileName == string.Empty? cOriginalName:cNewFileName;
            this.oLog.LogInfo("File name: " + cNewFileName);
            return cNewFileName;
        }


        private bool bMoveFile(ooFtpBO oFtpIn, string sOriginalPath, String sNewPath, string sFile, ooInfoDocumentoBE oInfoDoc)
        {
            bool bResp = false;
            try
            {
                ooFileBO oFiles = new ooFileBO(); //For combine and remove temporary files.
                oFtpIn.renameShell(oInfoDoc,sFile, oFiles.sPathCombine(sNewPath, sFile));
                oFiles.moveFile(oFiles.sPathCombineWin(System.AppDomain.CurrentDomain.BaseDirectory, sFile), oFiles.sPathCombineWin(System.AppDomain.CurrentDomain.BaseDirectory, System.Configuration.ConfigurationManager.AppSettings["sInternalTempDirectory"]));
                bResp = true;
            }
            catch (Exception ex)
            {
                this.oLog.LogError(ex.Message);
            }
            return bResp;
        }

        /// <summary>
        /// Identify in file name, Type and Number of tributary document
        /// </summary>
        /// <param name="cDocType">Document type identified</param>
        /// <param name="cDocNumber">Document number identified</param>
        /// <param name="cFileName">File name to be used</param>
        /// <returns></returns>
        private bool bDocumentInfo(out string cDocType, out string cDocNumber, string cFileName)
        {
            bool bResp = true;
            if (cFileName.Contains('_'))
            {
                int index = cFileName.IndexOf('_');
                if (index != 0)
                {
                    if(cFileName.Contains('.')){
                    int indexEnd = cFileName.IndexOf('.') - (index + 1);
                    cDocType = cFileName.Substring(0, index);
                    cDocNumber = cFileName.Substring(index + 1, indexEnd);
                    }else{
                        cDocType = cFileName.Substring(0, index);
                        cDocNumber = cFileName.Substring(index + 1, cFileName.Length - (index +1));
                    }
                }
                else
                {
                    cDocType = string.Empty;
                    cDocNumber = string.Empty;
                    bResp = false;
                }
            }
            else
            {
                cDocType = string.Empty;
                cDocNumber = string.Empty;
                bResp = false;
            }
            return bResp;
        }


    }
}
