using BLL;
using Common;
using Manager;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Http;

namespace RESTApi
{


    public class DataController : ApiController
    {

        public string GetAllCategories()
        {

            string jsonMessage = null;
            List<Category> ret = CategoryServices.GetAll().ToList();
            try
            {


                var settings = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                jsonMessage = JsonConvert.SerializeObject(ret, settings);


            }
            catch(Exception e)
            {

            }

            return jsonMessage;
        }



        [HttpPost]
        public int CheckOrder([FromBody] SaleOrderCommand command)
        {
            return 0;
            if (OtomatManager.GetInstance().allConnectionsOK == true)
            {
                return StockServices.CheckOrder(command);
            }
            else
            {
                return -1;
            }


        }


        [HttpGet]
        public int DoTest()
        {
            return 1234;
        }
    }
}
