using System;
using System.Runtime.InteropServices;
//using System.s;
using System.Threading;
using System.ServiceProcess;


/// <summary>
/// Manage Services from C# code
/// code borrowed from:
/// http://stackoverflow.com/questions/358700/how-to-install-a-windows-service-programmatically-in-c
/// This code was already tested in several production environments, and can be considered as stable.
/// </summary>
public static class ServiceManager
{
    private const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
    private const int SERVICE_WIN32_OWN_PROCESS = 0x00000010;

    [StructLayout(LayoutKind.Sequential)]
    private class SERVICE_STATUS
    {
        public int dwServiceType = 0;
        public ServiceState dwCurrentState = 0;
        public int dwControlsAccepted = 0;
        public int dwWin32ExitCode = 0;
        public int dwServiceSpecificExitCode = 0;
        public int dwCheckPoint = 0;
        public int dwWaitHint = 0;
    }

    #region OpenSCManager
    /// <summary>
    /// Opens the SC manager.
    /// </summary>
    /// <param name="machineName">Name of the machine.</param>
    /// <param name="databaseName">Name of the database.</param>
    /// <param name="dwDesiredAccess">The dw desired access.</param>
    /// <returns></returns>
    [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
    static extern IntPtr OpenSCManager(string machineName, string databaseName, ScmAccessRights dwDesiredAccess);
    #endregion

    #region OpenService
    /// <summary>
    /// Opens the service.
    /// </summary>
    /// <param name="hSCManager">The h SC manager.</param>
    /// <param name="lpServiceName">Name of the lp service.</param>
    /// <param name="dwDesiredAccess">The dw desired access.</param>
    /// <returns></returns>
    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, ServiceAccessRights dwDesiredAccess);
    #endregion

    #region CreateService
    /// <summary>
    /// Creates the service.
    /// </summary>
    /// <param name="hSCManager">The h SC manager.</param>
    /// <param name="lpServiceName">Name of the lp service.</param>
    /// <param name="lpDisplayName">Display name of the lp.</param>
    /// <param name="dwDesiredAccess">The dw desired access.</param>
    /// <param name="dwServiceType">Type of the dw service.</param>
    /// <param name="dwStartType">Start type of the dw.</param>
    /// <param name="dwErrorControl">The dw error control.</param>
    /// <param name="lpBinaryPathName">Name of the lp binary path.</param>
    /// <param name="lpLoadOrderGroup">The lp load order group.</param>
    /// <param name="lpdwTagId">The LPDW tag id.</param>
    /// <param name="lpDependencies">The lp dependencies.</param>
    /// <param name="lp">The lp.</param>
    /// <param name="lpPassword">The lp password.</param>
    /// <returns></returns>
    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr CreateService(IntPtr hSCManager, string lpServiceName, string lpDisplayName, ServiceAccessRights dwDesiredAccess, int dwServiceType, ServiceBootFlag dwStartType, ServiceError dwErrorControl, string lpBinaryPathName, string lpLoadOrderGroup, IntPtr lpdwTagId, string lpDependencies, string lp, string lpPassword);
    #endregion

    #region CloseServiceHandle
    /// <summary>
    /// Closes the service handle.
    /// </summary>
    /// <param name="hSCObject">The h SC object.</param>
    /// <returns></returns>
    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool CloseServiceHandle(IntPtr hSCObject);
    #endregion

    #region QueryServiceStatus
    [DllImport("advapi32.dll")]
    private static extern int QueryServiceStatus(IntPtr hService, SERVICE_STATUS lpServiceStatus);
    #endregion

    #region DeleteService
    /// <summary>
    /// Deletes the service.
    /// </summary>
    /// <param name="hService">The h service.</param>
    /// <returns></returns>
    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteService(IntPtr hService);
    #endregion

    #region ControlService
    /// <summary>
    /// Controls the service.
    /// </summary>
    /// <param name="hService">The h service.</param>
    /// <param name="dwControl">The dw control.</param>
    /// <param name="lpServiceStatus">The lp service status.</param>
    /// <returns></returns>
    [DllImport("advapi32.dll")]
    private static extern int ControlService(IntPtr hService, ServiceControl dwControl, SERVICE_STATUS lpServiceStatus);
    #endregion

    #region StartService
    /// <summary>
    /// Starts the service.
    /// </summary>
    /// <param name="hService">The h service.</param>
    /// <param name="dwNumServiceArgs">The dw num service args.</param>
    /// <param name="lpServiceArgVectors">The lp service arg vectors.</param>
    /// <returns></returns>
    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern int StartService(IntPtr hService, int dwNumServiceArgs, int lpServiceArgVectors);
    #endregion


    /// <summary>
    /// Indicates if the service could be
    /// successfully started.
    /// </summary>
    public static bool ServiceSuccessfullyStarted;


    /// <summary>
    /// Uninstalls the specified service name.
    /// </summary>
    /// <param name="serviceName">Name of the service.</param>
    public static void Uninstall(string serviceName)
    {
        IntPtr scm = OpenSCManager(ScmAccessRights.AllAccess);

        try
        {
            IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.AllAccess);
            if (service == IntPtr.Zero)
            {
                Console.WriteLine("Service not installed.");
                return;
            }

            try
            {
                StopService(service);
                if (!DeleteService(service))
                {
                    Console.WriteLine("Could not delete service " + Marshal.GetLastWin32Error());
                    return;
                }
            }
            finally
            {
                CloseServiceHandle(service);
            }
        }
        finally
        {
            CloseServiceHandle(scm);
        }
    }

    /// <summary>
    /// Check, if service is installed.
    /// </summary>
    /// <param name="serviceName">Name of the service.</param>
    /// <returns></returns>
    public static bool ServiceIsInstalled(string serviceName)
    {
        IntPtr scm = OpenSCManager(ScmAccessRights.Connect);

        try
        {
            IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryStatus);

            if (service == IntPtr.Zero)
                return false;

            CloseServiceHandle(service);
            return true;
        }
        finally
        {
            CloseServiceHandle(scm);
        }
    }

    /// <summary>
    /// Installs and start the service.
    /// </summary>
    /// <param name="serviceName">Name of the service.</param>
    /// <param name="displayName">The display name.</param>
    /// <param name="fileName">Name of the file.</param>
    public static void InstallAndStart(string serviceName, string displayName, string fileName)
    {
        IntPtr scm = OpenSCManager(ScmAccessRights.AllAccess);

        try
        {
            IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.AllAccess);

            if (service == IntPtr.Zero)
                service = CreateService(scm, serviceName, displayName, ServiceAccessRights.AllAccess, SERVICE_WIN32_OWN_PROCESS, ServiceBootFlag.AutoStart, ServiceError.Normal, fileName, null, IntPtr.Zero, null, null, null);

            if (service == IntPtr.Zero)
            {
                Console.WriteLine("Failed to install service.");
                return;
            }

            try
            {
                StartService(service);
            }
            finally
            {
                CloseServiceHandle(service);
            }
        }
        finally
        {
            CloseServiceHandle(scm);
        }
    }

    /// <summary>
    /// Starts the service.
    /// </summary>
    /// <param name="serviceName">Name of the service.</param>
    public static void StartService(string serviceName)
    {
        IntPtr scm = OpenSCManager(ScmAccessRights.Connect);

        try
        {
            IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryStatus | ServiceAccessRights.Start);
            if (service == IntPtr.Zero)
            {
                Console.WriteLine("Could not open service.");
                return;
            }
            else
            {
                ServiceSuccessfullyStarted = false;
            }

            try
            {
                StartService(service);
                ServiceSuccessfullyStarted = true;
            }
            finally
            {
                CloseServiceHandle(service);
            }
        }
        finally
        {
            CloseServiceHandle(scm);
        }
    }

    /// <summary>
    /// Stops the service.
    /// </summary>
    /// <param name="serviceName">Name of the service.</param>
    public static void StopService(string serviceName)
    {
        IntPtr scm = OpenSCManager(ScmAccessRights.Connect);

        try
        {
            IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryStatus | ServiceAccessRights.Stop);
            if (service == IntPtr.Zero)
            {
                Console.WriteLine("Could not open service.");
                return;
            }

            try
            {
                StopService(service);
            }
            finally
            {
                CloseServiceHandle(service);
            }
        }
        finally
        {
            CloseServiceHandle(scm);
        }
    }

    /// <summary>
    /// Starts the service.
    /// </summary>
    /// <param name="service">The service.</param>
    private static void StartService(IntPtr service)
    {
        SERVICE_STATUS status = new SERVICE_STATUS();
        int i = StartService(service, 0, 0);
        var changedStatus = WaitForServiceStatus(service, ServiceState.StartPending, ServiceState.Running);
        ServiceSuccessfullyStarted = true;
        if (!changedStatus)
        {
            ServiceSuccessfullyStarted = true;
            Console.WriteLine("Unable to start service");
        }
    }

    /// <summary>
    /// Stops the service.
    /// </summary>
    /// <param name="service">The service.</param>
    private static void StopService(IntPtr service)
    {
        SERVICE_STATUS status = new SERVICE_STATUS();
        ControlService(service, ServiceControl.Stop, status);
        var changedStatus = WaitForServiceStatus(service, ServiceState.StopPending, ServiceState.Stopped);
        if (!changedStatus)
            Console.WriteLine("Unable to stop service");
    }

    /// <summary>
    /// Gets the service status.
    /// </summary>
    /// <param name="serviceName">Name of the service.</param>
    /// <returns></returns>
    public static ServiceState GetServiceStatus(string serviceName)
    {
        IntPtr scm = OpenSCManager(ScmAccessRights.Connect);

        try
        {
            IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryStatus);
            if (service == IntPtr.Zero)
                return ServiceState.NotFound;

            try
            {
                return GetServiceStatus(service);
            }
            finally
            {
                CloseServiceHandle(service);
            }
        }
        finally
        {
            CloseServiceHandle(scm);
        }
    }




    /// <summary>
    /// Gets the service status.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <returns></returns>
    private static ServiceState GetServiceStatus(IntPtr service)
    {
        SERVICE_STATUS status = new SERVICE_STATUS();

        if (QueryServiceStatus(service, status) == 0)
            Console.WriteLine("Failed to query service status.");

        return status.dwCurrentState;
    }

    /// <summary>
    /// Waits for service status.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="waitStatus">The wait status.</param>
    /// <param name="desiredStatus">The desired status.</param>
    /// <returns></returns>
    private static bool WaitForServiceStatus(IntPtr service, ServiceState waitStatus, ServiceState desiredStatus)
    {
        SERVICE_STATUS status = new SERVICE_STATUS();

        QueryServiceStatus(service, status);
        if (status.dwCurrentState == desiredStatus) return true;

        int dwStartTickCount = Environment.TickCount;
        int dwOldCheckPoint = status.dwCheckPoint;

        while (status.dwCurrentState == waitStatus)
        {
            // Do not wait longer than the wait hint. A good interval is
            // one tenth the wait hint, but no less than 1 second and no
            // more than 10 seconds.

            int dwWaitTime = status.dwWaitHint / 10;

            if (dwWaitTime < 1000) dwWaitTime = 1000;
            else if (dwWaitTime > 10000) dwWaitTime = 10000;

            Thread.Sleep(dwWaitTime);

            // Check the status again.

            if (QueryServiceStatus(service, status) == 0) break;

            if (status.dwCheckPoint > dwOldCheckPoint)
            {
                // The service is making progress.
                dwStartTickCount = Environment.TickCount;
                dwOldCheckPoint = status.dwCheckPoint;
            }
            else
            {
                if (Environment.TickCount - dwStartTickCount > status.dwWaitHint)
                {
                    // No progress made within the wait hint
                    break;
                }
            }
        }
        return (status.dwCurrentState == desiredStatus);
    }

    /// <summary>
    /// Opens the SC manager.
    /// </summary>
    /// <param name="rights">The rights.</param>
    /// <returns></returns>
    private static IntPtr OpenSCManager(ScmAccessRights rights)
    {
        IntPtr scm = OpenSCManager(null, null, rights);
        if (scm == IntPtr.Zero)
        {
            int error = Marshal.GetLastWin32Error();
            Console.WriteLine("Could not connect to service control manager.");
        }

        return scm;
    }

    /// <summary>
    /// Pauses the service.
    /// </summary>
    /// <param name="serviceName">Name of the service.</param>
    /// <returns></returns>
    public static bool PauseService(string serviceName)
    {
        using (ServiceController sc = new ServiceController(serviceName))
        {
            if (sc.CanPauseAndContinue)
            {
                sc.Pause();

                while (sc.Status != ServiceControllerStatus.Paused)
                {
                    Thread.Sleep(1000);
                    sc.Refresh();
                }

                if (sc.Status == ServiceControllerStatus.Paused)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Continues the service.
    /// </summary>
    /// <param name="serviceName">Name of the service.</param>
    /// <returns></returns>
    public static bool ContinueService(string serviceName)
    {
        using (ServiceController sc = new ServiceController(serviceName))
        {
            if (sc.CanPauseAndContinue)
            {
                sc.Continue();

                while (sc.Status != ServiceControllerStatus.ContinuePending)
                {
                    Thread.Sleep(1000);
                    sc.Refresh();
                }

                if (sc.Status == ServiceControllerStatus.Running)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        return false;
    }
}


public enum ServiceState
{
    Unknown = -1, // The state cannot be (has not been) retrieved.
    NotFound = 0, // The service is not known on the host server.
    Stopped = 1,
    StartPending = 2,
    StopPending = 3,
    Running = 4,
    ContinuePending = 5,
    PausePending = 6,
    Paused = 7
}

[Flags]
public enum ScmAccessRights
{
    Connect = 0x0001,
    CreateService = 0x0002,
    EnumerateService = 0x0004,
    Lock = 0x0008,
    QueryLockStatus = 0x0010,
    ModifyBootConfig = 0x0020,
    StandardRightsRequired = 0xF0000,
    AllAccess = (StandardRightsRequired | Connect | CreateService |
                 EnumerateService | Lock | QueryLockStatus | ModifyBootConfig)
}

[Flags]
public enum ServiceAccessRights : uint
{
    QueryConfig = 0x1,
    ChangeConfig = 0x2,
    QueryStatus = 0x4,
    EnumerateDependants = 0x8,
    Start = 0x10,
    Stop = 0x20,
    PauseContinue = 0x40,
    Interrogate = 0x80,
    UserDefinedControl = 0x100,
    Delete = 0x00010000,
    StandardRightsRequired = 0xF0000,
    AllAccess = (StandardRightsRequired | QueryConfig | ChangeConfig |
                 QueryStatus | EnumerateDependants | Start | Stop | PauseContinue |
                 Interrogate | UserDefinedControl)
}

public enum ServiceBootFlag
{
    Start = 0x00000000,
    SystemStart = 0x00000001,
    AutoStart = 0x00000002,
    DemandStart = 0x00000003,
    Disabled = 0x00000004
}

public enum ServiceControl
{
    Stop = 0x00000001,
    Pause = 0x00000002,
    Continue = 0x00000003,
    Interrogate = 0x00000004,
    Shutdown = 0x00000005,
    ParamChange = 0x00000006,
    NetBindAdd = 0x00000007,
    NetBindRemove = 0x00000008,
    NetBindEnable = 0x00000009,
    NetBindDisable = 0x0000000A
}

public enum ServiceError
{
    Ignore = 0x00000000,
    Normal = 0x00000001,
    Severe = 0x00000002,
    Critical = 0x00000003
}