using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;

namespace FitmediaServiceInstaller
{
    class ServiceInstaller
    {
        #region Private Variables
        private string _servicePath;
        private string _serviceName;
        private string _serviceDisplayName;
        #endregion Private Variables
        #region DLLImport
        [DllImport("advapi32.dll")]
        public static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, int scParameter);
        [DllImport("Advapi32.dll")]
        public static extern IntPtr CreateService(IntPtr SC_HANDLE, string lpSvcName, string lpDisplayName,
        int dwDesiredAccess, int dwServiceType, int dwStartType, int dwErrorControl, string lpPathName,
        string lpLoadOrderGroup, int lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);
        [DllImport("advapi32.dll")]
        public static extern void CloseServiceHandle(IntPtr SCHANDLE);
        [DllImport("advapi32.dll")]
        public static extern int StartService(IntPtr SVHANDLE, int dwNumServiceArgs, string lpServiceArgVectors);
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr SCHANDLE, string lpSvcName, int dwNumServiceArgs);
        [DllImport("advapi32.dll")]
        public static extern int DeleteService(IntPtr SVHANDLE);
        [DllImport("kernel32.dll")]
        public static extern int GetLastError();
        #endregion DLLImport

        /// <summary>
        /// This method installs and runs the service in the service control manager.
        /// </summary>
        /// <param name="svcPath">The complete path of the service.</param>
        /// <param name="svcName">Name of the service.</param>
        /// <param name="svcDispName">Display name of the service.</param>
        /// <returns>True if the process went thro successfully. False if there was any error.</returns>
        public bool InstallService(string svcPath, string svcName, string svcDispName)
        {
            ServiceManager.InstallAndStart(svcName, svcDispName, svcPath);
            return true;
            #region Constants declaration.
            int SC_MANAGER_CREATE_SERVICE = 0x0002;
            int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
            //int SERVICE_DEMAND_START = 0x00000003;
            int SERVICE_ERROR_NORMAL = 0x00000001;
            int STANDARD_RIGHTS_REQUIRED = 0xF0000;
            int SERVICE_QUERY_CONFIG = 0x0001;
            int SERVICE_CHANGE_CONFIG = 0x0002;
            int SERVICE_QUERY_STATUS = 0x0004;
            int SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
            int SERVICE_START = 0x0010;
            int SERVICE_STOP = 0x0020;
            int SERVICE_PAUSE_CONTINUE = 0x0040;
            int SERVICE_INTERROGATE = 0x0080;
            int SERVICE_USER_DEFINED_CONTROL = 0x0100;
            int SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
            SERVICE_QUERY_CONFIG |
            SERVICE_CHANGE_CONFIG |
            SERVICE_QUERY_STATUS |
            SERVICE_ENUMERATE_DEPENDENTS |
            SERVICE_START |
            SERVICE_STOP |
            SERVICE_PAUSE_CONTINUE |
            SERVICE_INTERROGATE |
            SERVICE_USER_DEFINED_CONTROL);
            int SERVICE_AUTO_START = 0x00000002;
            #endregion Constants declaration.
            try
            {
                IntPtr sc_handle = OpenSCManager(null, null, SC_MANAGER_CREATE_SERVICE);
                if (sc_handle.ToInt32() != 0)
                {
                    IntPtr sv_handle = CreateService(sc_handle, svcName, svcDispName, SERVICE_ALL_ACCESS, SERVICE_WIN32_OWN_PROCESS, SERVICE_AUTO_START, SERVICE_ERROR_NORMAL, svcPath, null, 0, null, null, null);
                    if (sv_handle.ToInt32() == 0)
                    {
                        CloseServiceHandle(sc_handle);
                        Console.WriteLine("Couldnt create service '" + svcName + "'");
                        return false;
                    }
                    else
                    {
                        //now trying to start the service
                        int i = StartService(sv_handle, 0, null);
                        // If the value i is zero, then there was an error starting the service.
                        // note: error may arise if the service is already running or some other problem.
                        if (i == 0)
                        {
                            Console.WriteLine("Couldnt start service '" + svcName + "'");
                            return false;
                        }
                        //Console.WriteLine("Success");
                        CloseServiceHandle(sc_handle);
                        Console.WriteLine("Service '" + svcName + "' was successfully created and started");
                        return true;
                    }
                }
                else
                    Console.WriteLine("SCM not opened successfully");
                    return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }
        /// <summary>
        /// This method uninstalls the service from the service conrol manager.
        /// </summary>
        /// <param name="svcName">Name of the service to uninstall.</param>
        public bool UnInstallService(string svcName)
        {
            ServiceManager.Uninstall(svcName);
            return false;
            int GENERIC_WRITE = 0x40000000;
            IntPtr sc_hndl = OpenSCManager(null, null, GENERIC_WRITE);
            if (sc_hndl.ToInt32() != 0)
            {
                int DELETE = 0x10000;
                IntPtr svc_hndl = OpenService(sc_hndl, svcName, DELETE);
                //Console.WriteLine(svc_hndl.ToInt32());
                if (svc_hndl.ToInt32() != 0)
                {
                    
		      /*  SERVICE_STATUS ss;

                    if( !::ControlService( sc_hndl, SERVICE_CONTROL_STOP, &ss ) )
		        {
			        DWORD dwErrCode = GetLastError(); 

			        if( dwErrCode != ERROR_SERVICE_NOT_ACTIVE )
			        {
				        return false;
			        }
		        }


		        // Wait until it stopped (or timeout expired)

		        if( !WaitForServiceToReachState( hService, SERVICE_STOPPED, &ss, INFINITE ) )
		        {
			        return false;
		        }*/


                    int i = DeleteService(svc_hndl);
                    if (i != 0)
                    {
                        CloseServiceHandle(sc_hndl);
                        Console.WriteLine("Service '" + svcName + "' successfully deleted");
                        return true;
                    }
                    else
                    {
                        CloseServiceHandle(sc_hndl);
                        Console.WriteLine("Couldnt delete service '" + svcName + "'");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Couldnt open service '" + svcName + "'");
                    return false;
                }
            }
            else
                return false;
        }


    }

    class Program
    {
 
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        #region Main method + testing code
        [STAThread]
        static void Main(string[] args)
        {
        // TODO: Add code to start application here
        #region Testing
        // Testing --------------
        string svcPath;
        string svcName;
        string svcDispName;
        //path to the service that you want to install
        //svcPath = @"D:\Haik\Vigen\fitmedia\FitmediaSync\bin\Debug\FitmediaSync.exe";
        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
        svcPath = path.Replace("file:\\", "") + "\\FitmediaSync.exe";
        svcDispName="Fitmedia Syncronization";
        svcName= "Fitmedia Sync";
        Console.Write("Select action (i-install, u-uninstall): ");
        char ch = Console.ReadKey().KeyChar;
        ServiceInstaller c = new ServiceInstaller();
        switch(ch)
        {
            case 'i': c.InstallService(svcPath, svcName, svcDispName); break;
            case 'u': c.UnInstallService(svcName); break;
        }
        Console.Write("Press any key to close...");
        Console.ReadKey();
        //Testing --------------
        #endregion Testing
        }
        #endregion Main method + testing code - Commented
        
    }
}
