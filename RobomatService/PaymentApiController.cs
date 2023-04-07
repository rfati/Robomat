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

namespace RobomatService
{


    public class PaymentController : ApiController
    {

       
        [HttpPost]
        public int DoSatis([FromBody] SaleOrderCommand command)
        {
            return OtomatManager.GetInstance().DoSatisIslem(command);
        }

    }
}
