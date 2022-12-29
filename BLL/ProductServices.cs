using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Model;

namespace BLL
{
    public class ProductServices
    {


        public static void Delete(Product obj)
        {
            using (RobomatUnitOfWork worker = new RobomatUnitOfWork())
            {
                worker.ProductRepository.Delete(obj);
            }
        }

        public static IEnumerable<Product> GetAll()
        {
            using (RobomatUnitOfWork worker = new RobomatUnitOfWork())
            {
                var ret = worker.ProductRepository.GetAll(o => o.RafBolmes).ToList();
                return ret;
            }
        }

        public static Product GetById(int id)
        {
            using (RobomatUnitOfWork worker = new RobomatUnitOfWork())
            {
                var products = worker.ProductRepository.GetAll(o => o.RafBolmes).ToList();
                var ret = products.Where(o => o.ProductId == id).FirstOrDefault();
                return ret;
            }
        }

        public static void Insert(Product obj)
        {
            using (RobomatUnitOfWork worker = new RobomatUnitOfWork())
            {
                worker.ProductRepository.Insert(obj);
            }
        }

        public static void Update(Product product, RafBolme raf)
        {
            using (RobomatUnitOfWork worker = new RobomatUnitOfWork())
            {
                worker.ProductRepository.Update(product);
                worker.RafBolmeRepository.Update(raf);
            }
        }
    }
}
