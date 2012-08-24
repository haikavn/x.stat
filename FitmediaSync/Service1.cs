using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using APIServiceProviderNamespace;
using System.Threading;
using System.Reflection;
using System.IO;
//using System.Windows.Forms;

namespace FitmediaSync
{
    public partial class FitmediaSync : ServiceBase
    {

        SyncManager sm = new SyncManager();


        public FitmediaSync()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            File.WriteAllText(path.Replace("file:\\", "") + "\\shutdown", "ok");

            //System.Windows.Forms.MessageBox.Show("kuku");

            sm.Start();
        }

        protected override void OnStop()
        {
            sm.Stop();

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            if (File.Exists(path.Replace("file:\\", "") + "\\shutdown"))
                File.Delete(path.Replace("file:\\", "") + "\\shutdown");
        }

        protected override void OnContinue()
        {
            base.OnContinue();
        }

        protected override void OnShutdown()
        {
            sm.Stop();

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            if (File.Exists(path.Replace("file:\\", "") + "\\shutdown"))
                File.Delete(path.Replace("file:\\", "") + "\\shutdown");


         //   System.Windows.Forms.MessageBox.Show("haha");

            base.OnShutdown();
        }

    }
}
