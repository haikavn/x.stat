using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace FitmediaSync
{
    static class Program
    {
        private static void InstallService()
        {
            string svcPath;
            string svcName;
            string svcDispName;
            //path to the service that you want to install
            svcPath = @"D:\Haik\Vigen\fitmedia\FitmediaSync\bin\Debug\FitmediaSync.exe";
            svcDispName = "Fitmedia Syncronization";
            svcName = "Fitmedia Sync";
            FitmediaServiceInstaller c = new FitmediaServiceInstaller();
            c.UnInstallService(svcName);
            c.InstallService(svcPath, svcName, svcDispName);
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

            //InstallService();
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new FitmediaSync() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
