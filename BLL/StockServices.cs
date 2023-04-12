using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using DAL;
using Model;

namespace BLL
{
    public class StockServices
    {
        public static int CheckOrder(SaleOrderCommand command)
        {
            var groupedCartItemList = command.SaleOrder.GroupBy(u => u.ProductId).Select(grp => grp.ToList()).ToList();

            //foreach (var CartItemList in groupedCartItemList)
            //{
            //    int TotalOrderQuantityofProduct = 0;
            //    int productId = CartItemList[0].ProductId;
            //    foreach (var CartItem in CartItemList)
            //    {
            //        TotalOrderQuantityofProduct = TotalOrderQuantityofProduct + CartItem.Quantity;

            //    }

            //    Product product = ProductServices.GetById(productId);

            //    //if (TotalOrderQuantityofProduct > product.StockAdet)
            //    //{
            //    //    return 1;
            //    //}
            //}

            return 0;

        }



    }
}
