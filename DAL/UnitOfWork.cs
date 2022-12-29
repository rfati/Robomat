using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Transactions;

namespace DAL
{
    public interface IUnitOfWork
            : IDisposable
    {
        void Save();
        // Başka operasyonlar da tanımlanabilir. 
        // void OpenTransaction(); 
        // void CloseTransaction(); 
        // gibi 
    }

    public class RobomatUnitOfWork
        : IUnitOfWork
    {
        private RobomatContext _context = new RobomatContext();
        private RobomatRepository<Category> _categoryRepository;
        private RobomatRepository<Product> _productRepository;
        private RobomatRepository<RafBolme> _rafBolmeRepository;
        private bool _disposed = false;
        public RobomatRepository<Category> CategoryRepository
        {
            get
            {
                if (_categoryRepository == null)
                    _categoryRepository = new RobomatRepository<Category>(_context);
                return _categoryRepository;
            }
        }
        public RobomatRepository<Product> ProductRepository
        {
            get
            {
                if (_productRepository == null)
                    _productRepository = new RobomatRepository<Product>(_context);
                return _productRepository;
            }
        }

        public RobomatRepository<RafBolme> RafBolmeRepository
        {
            get
            {
                if (_rafBolmeRepository == null)
                    _rafBolmeRepository = new RobomatRepository<RafBolme>(_context);
                return _rafBolmeRepository;
            }
        }

        public void Save()
        {
            using (TransactionScope tScope = new TransactionScope())
            {
                _context.SaveChanges();
                tScope.Complete();
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
