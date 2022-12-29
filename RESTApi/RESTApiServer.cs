using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTApi
{
    public class RESTApiServer
    {

        private static RESTApiServer _instance;
        private static object syncLock = new object();

        protected RESTApiServer()
        {
        }

        public static RESTApiServer GetInstance()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new RESTApiServer();
                    }
                }
            }

            return _instance;

        }


        public void Start()
        {

            const string url = "http://localhost:9002";
            WebApp.Start<RESTStartup>(url);
            //using (WebApp.Start<RESTStartup>(url))
            //{
            //    Console.WriteLine("Server started at:" + url);
            //}


        }
    }
}
