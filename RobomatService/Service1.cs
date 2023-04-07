
using Common;
using Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace RobomatService
{
    public partial class Service1 : ServiceBase
    {
        Timer aTimer;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            var config = new HttpSelfHostConfiguration("http://localhost:9002");

            config.Routes.MapHttpRoute(
               name: "API",
               routeTemplate: "api/{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );

            HttpSelfHostServer server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();


            aTimer = new Timer(1000);
            aTimer.AutoReset = false;
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Enabled = true;


        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                OtomatManager.GetInstance();
                OtomatManager.GetInstance().Init();
                OtomatManager.GetInstance().Start();
                OtomatManager.GetInstance().SetMode(Mode.SaleService);
            }
            catch (Exception ex)
            {
                Logger.LogError("exception:  " + ex.Message);
            }

        }

        protected override void OnStop()
        {
        }
    }
}
