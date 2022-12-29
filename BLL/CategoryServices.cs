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
    public class CategoryServices
    {


        public static void Delete(Category obj)
        {
            using (RobomatUnitOfWork worker = new RobomatUnitOfWork())
            {
                worker.CategoryRepository.Delete(obj);
            }
        }

        public static IEnumerable<Category> GetAll()
        {
            using (RobomatUnitOfWork worker = new RobomatUnitOfWork())
            {
                var ret = worker.CategoryRepository.GetAll(o=>o.Products).ToList();
                return ret;
            }
        }

        public static Category GetById(int id)
        {
            using (RobomatUnitOfWork worker = new RobomatUnitOfWork())
            {
                var ret = worker.CategoryRepository.FindById(id);
                return ret;
            }
        }

        public static void Insert(Category obj)
        {
            using (RobomatUnitOfWork worker = new RobomatUnitOfWork())
            {
                worker.CategoryRepository.Insert(obj);
            }
        }

        public static void Update(Category obj)
        {
            using (RobomatUnitOfWork worker = new RobomatUnitOfWork())
            {
                worker.CategoryRepository.Update(obj);
            }
        }
    }
}
