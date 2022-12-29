using DAL;
using Manager;
using RESTApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RobomatService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            using (RobomatUnitOfWork worker = new RobomatUnitOfWork())
            {

                var books = worker.CategoryRepository.FindById(1);

            }

            RESTApiServer rESTApiServer = RESTApiServer.GetInstance();
            rESTApiServer.Start();

            OtomatManager.GetInstance();
            OtomatManager.GetInstance().Init();
            OtomatManager.GetInstance().Start();
        }

        protected override void OnStop()
        {
        }
    }
}
