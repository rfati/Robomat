using BLL;
using Common;
using Manager;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace RESTApi
{


    public class ServiceController : ApiController
    {

        [HttpPost]
        public int DoService([FromBody] SaleOrderCommand command)
        {
            return OtomatManager.GetInstance().DoServisIslem(command);
        }

    }
}
