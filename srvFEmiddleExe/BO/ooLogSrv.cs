using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using comBusinessBE.BI;
using System.Diagnostics;

namespace srvFEmiddleExe.BO
{
    public class ooLogSrv : comBusinessBE.BI.ILog
    {
        #region atributos privados
            private log4net.ILog _oLog;
            private String _sSourceEvLog = "srvFEmiddle.Servicio";
        #endregion

        public ooLogSrv()
        {
            string sBasePath = System.AppDomain.CurrentDomain.BaseDirectory;
            try
            {
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(Path.Combine(sBasePath, "Log4Net.config.xml")));
                _oLog = LogManager.GetLogger(typeof(comBusinessBE.BI.IOperacion).Name);
            }
            catch (Exception ex){}
            finally
            {
                if(!EventLog.SourceExists(_sSourceEvLog))
                {
                    //Si no es posible escribir en el log, se registra en el log de eventos.
                    EventLog.CreateEventSource(_sSourceEvLog, "Application");
                }
            }
        }

        public void LogException(object oEx)
        {
            try
            {
                this._oLog.Fatal(oEx);
            }
            catch (Exception ex)
            {
                EventLog ELog = new EventLog("Application", ".", this._sSourceEvLog);
                ELog.WriteEntry(String.Format("No fue posible registrar la Excepcion: {0}, en log4net", oEx));
            }
        }

        public void LogInfo(object oInfo)
        {
            try
            {
                this._oLog.Info(oInfo);
            }
            catch (Exception ex)
            {
                EventLog ELog = new EventLog("Application", ".", this._sSourceEvLog);
                ELog.WriteEntry(String.Format("No fue posible registrar la Excepcion: {0}, en log4net", oInfo));
            }
        }

        public void LogError(object oError)
        {
            try
            {
                this._oLog.Error(oError);
            }
            catch (Exception ex)
            {
                EventLog ELog = new EventLog("Application", ".", this._sSourceEvLog);
                ELog.WriteEntry(String.Format("No fue posible registrar Error: {0}, en log4net", oError));
            }
        }
    }
}
