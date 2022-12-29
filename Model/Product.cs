using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Product
    {
        public Product()
        {
            this.RafBolmes = new HashSet<RafBolme>();
        }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<RafBolme> RafBolmes { get; set; }
        public int ProductNo { get; set; }
        public string Name { get; set; }
        public double BasePrice { get; set; }
        public int DiscountValue { get; set; }
        public int StockAdet { get; set; }
        public string UIProductPicPath { get; set; }
        public string UIAmbalajPicPath { get; set; }
        public string Mensei { get; set; }
        public string Icindekiler { get; set; }
        public string AlerjenUyarı { get; set; }
        public short ServisType { get; set; }
        public int UIOrder { get; set; }
        public AmbalajType PackageType { get; set; }
        public KapType KapType { get; set; }
        public KapakType KapakType { get; set; }

    }
}
