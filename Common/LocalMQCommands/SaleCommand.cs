using System.Collections.Generic;

namespace Common
{

    
    public class CartItemMessage
    {
        public int ProductId { get; set; }
        public short Quantity { get; set; }
        public ServiceType serviceType { get; set; }  //0:Package, 1:cold, 2:Hot
        public HotServiceType hotServiceType { get; set; }  //0:ilik, 1:orta, 2:sicak

    }
    public class SaleOrderCommand
    {
        public float TotalPrice { get; set; }
        public List<CartItemMessage> SaleOrder;

    }

}

