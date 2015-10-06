using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.IO;
using Spring.Context;
using Spring.Context.Support;
using srvFEmiddle.BO;
using comBusinessBE.BI;
using System.Configuration;

namespace srvFEmiddle
{
    partial class srvFEmiddle : ServiceBase
    {
        private Boolean _bEstaEjecutandose = true; //para avisar al thread el termino de ejecucion.
        private Thread _threadTraspasaDocs; // thread que ejecutara al realizar las Operaciones.
        private IApplicationContext _appContext; //Contexto spring.
        private static int _SEGUNDOS_ESPERA_PORDEF = 30;
        private static String _sSourceEvLog = "srvFEmiddle.Servicio";

        public srvFEmiddle()
        {
            InitializeComponent();
        }
        
        public void init() { this.OnStart(new string[1] { "" }); } //veremos

        protected override void OnStart(string[] args)
        {
            ooLogSrv oLogSvc = null;
            try
            {
                ThreadStart ts = new ThreadStart(TraspasaDocumentos);
                oLogSvc = new ooLogSrv();
                if (!EventLog.SourceExists(_sSourceEvLog))
                    EventLog.CreateEventSource(_sSourceEvLog, "Application");

                this._threadTraspasaDocs = new Thread(ts);
                this._bEstaEjecutandose = true;

                this._threadTraspasaDocs.Start(); //Se inicia la ejecución del servicio
            }
            catch (Exception ex)
            {
                if(oLogSvc != null)
                    oLogSvc.LogException(String.Format("No fue posible inicial el servicio srvFEmiddle: {0}", ex));
            }

        }

        protected override void OnStop()
        {
            this._bEstaEjecutandose = false;
            this._threadTraspasaDocs.Join(new TimeSpan(0, 0, 30));
        }

        private void TraspasaDocumentos()
        {
            List<IOperacion> lstsrvmiddle = null;
            int nRt = 0;
            int nSegundosEspera = 30;
            ILog oLog = null;
            string sAux = string.Empty;

            EventLog ELog = new EventLog("Application", ".", _sSourceEvLog);
            String sBasePath = System.AppDomain.CurrentDomain.BaseDirectory;
            try
            {
                oLog = new ooLogSrv();
                ELog.WriteEntry(string.Format("Antes de config spring, base path: {0}", sBasePath));
                while (this._bEstaEjecutandose)
                {
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
                    oLog.LogInfo(string.Format("Servicio iniciado a las {0}", DateTime.Now));
                    nRt = 0;
                    sAux = ConfigurationManager.AppSettings["nSegundosEspera"].ToString();
                    if (string.IsNullOrEmpty(sAux) || !int.TryParse(sAux, out nSegundosEspera))
                    {
                        oLog.LogError(string.Format("Servicio no encontro la key 'nSegundosEspera' en app.config, se configura tiempo por defecto: {0}", _SEGUNDOS_ESPERA_PORDEF));
                        nSegundosEspera = _SEGUNDOS_ESPERA_PORDEF;
                    }
                    ELog.WriteEntry(string.Format("Servicio inicia proocesamiento a las '{0}'", DateTime.Now));
                    oLog.LogInfo(string.Format("Servicio inicia proocesamiento a las '{0}'", DateTime.Now));

                    try
                    {
                        foreach (IOperacion srvOp in lstsrvmiddle)
                        {
                            srvOp.oLog = oLog;
                            nRt = srvOp.lTraspasar();
                            oLog.LogInfo(string.Format("Servicio finaliza procesamiento de '{0}' registros del tipo {1} a las '{2}'", nRt, srvOp.GetType().Name, DateTime.Now));
                        }
                    }
                    catch (Exception ex)
                    {
                        oLog.LogException(ex);
                    }
                    if (nRt == 0 && this._bEstaEjecutandose) //si llegase a existir un contexto de conexion, aqui se debe reiniciar
                        Thread.Sleep(new TimeSpan(0, 0, nSegundosEspera));
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
