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
            sm.Start();
        }

        protected override void OnStop()
        {
            sm.Stop();
        }

    }
}
