using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Spring.Context;
using Spring.Context.Support;
using srvFEmiddleExe.BO;
using comBusinessBE.BI;
using System.Configuration;

namespace srvFEmiddleExe
{
    class srvFEmiddle
    {

        private IApplicationContext _appContext; //Contexto spring.

        private static String _sSourceEvLog = "srvFEmiddle.Exe";

        public srvFEmiddle()
        {
        }



        public void TraspasaDocumentos()
        {
            List<IOperacion> lstsrvmiddle = null;
            int nRt = 0;
            ILog oLog = null;
            string sAux = string.Empty;

            EventLog ELog = new EventLog("Application", ".", _sSourceEvLog);
            String sBasePath = System.AppDomain.CurrentDomain.BaseDirectory;
            try
            {
                oLog = new ooLogSrv();
                ELog.WriteEntry(string.Format("Antes de config spring, base path: {0}", sBasePath));

                    try
                    {
                        _appContext = new XmlApplicationContext(Path.Combine(sBasePath, "di.config.xml"));
                    }
                    catch (Exception ex)
                    {
                        oLog.LogException(ex);
                    }
                    ELog.WriteEntry(string.Format("Despues de config spring"));
                    lstsrvmiddle = (List<IOperacion>)_appContext.GetObject("pluginComponentesTraspasoDoc");
                    oLog.LogInfo(string.Format("Application started at  {0}", DateTime.Now));
                    nRt = 0;


                    ELog.WriteEntry(string.Format("Application started at '{0}'", DateTime.Now));

                    try
                    {
                        foreach (IOperacion srvOp in lstsrvmiddle)
                        {
                            srvOp.oLog = oLog;
                            nRt = srvOp.lTraspasar();
                            oLog.LogInfo(string.Format("Application finish procesing of '{0}' registers of  type {1} at '{2}'", nRt, srvOp.GetType().Name, DateTime.Now));
                        }
                    }
                    catch (Exception ex)
                    {
                        oLog.LogException(ex);
                    }
                   
                
            }
            catch (Exception ex)
            {
                ELog.WriteEntry(string.Format("Excepcion: {0}", ex));
                oLog.LogException(ex);
            }
            finally
            {
                oLog.LogInfo(string.Format("Servicio finaliza ejecucion a las '{0}'", DateTime.Now));
                Thread.CurrentThread.Abort(); //se finaliza el thread
            }
        }
    }
}
