using BLL;
using Common;
using Manager;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace RobomatService
{


    public class ServiceController : ApiController
    {

        [HttpPost]
        public int DoService([FromBody] SaleOrderCommand command)
        {
            Logger.LogInfo("Doservice UI komutu geldi.. productdID" + command.SaleOrder[0].ProductId);

            //Product product = ProductServices.GetById(command.SaleOrder[0].ProductId);
            //product.StockAdet--;
            //RafBolme raf = product.RafBolmes.First();
            //raf.StockAdet--;
            //ProductServices.Update(product, raf);

            //Thread.Sleep(10000);
            //return 0;

            return OtomatManager.GetInstance().DoServisIslem(command);
        }

    }
}
