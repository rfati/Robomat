using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Category
    {
        public Category()
        {
            this.Products = new HashSet<Product>();
        }
        public int CategoryId { get; set; }
        public int CategoryNo { get; set; }
        public string Name { get; set; }
        public string UICategoryPicPath { get; set; }
        public int UIRow { get; set; }
        public int UIColumn { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }

}
