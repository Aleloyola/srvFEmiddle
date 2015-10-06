using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace srvFEmiddle
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
            #if DEBUG
                //Se ejecuta como programa si se compila en debug
                srvFEmiddle srvModificador = new srvFEmiddle();
                srvModificador.init();
            #else
                //Se ejecuta como servicio si se compila en release
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			    { 
				    new srvFEmiddle() 
			    };
                ServiceBase.Run(ServicesToRun);
            #endif


        }
    }
}
