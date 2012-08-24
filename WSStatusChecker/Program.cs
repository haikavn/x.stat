using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Diagnostics;

namespace WSStatusChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServiceController sc = new ServiceController("Fitmedia Sync");

                switch (sc.Status)
                {
                    case ServiceControllerStatus.Running:
                        Console.WriteLine("Running");
                        break;
                    case ServiceControllerStatus.Stopped:
                        Console.WriteLine("Stopped"); break;
                    case ServiceControllerStatus.Paused:
                        Console.WriteLine("Paused"); break;
                    case ServiceControllerStatus.StopPending:
                        Console.WriteLine("Stopping"); break;
                    case ServiceControllerStatus.StartPending:
                        Console.WriteLine("Starting"); break;
                    default:
                        Console.WriteLine("Status Changing"); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.Write("Press any key to close...");
            Console.ReadKey();

        }
    }
}
