using SportsStore.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Concrete;

namespace SportsStore.Domain
{
    public class EFProductRepository : IProductRepository
    {
        private EFDbContext _context = new EFDbContext();

        public IEnumerable<Product> Products
        {
            get
            {
                return _context.Products;
            }
        }

        public void SaveProduct(Product product)
        {
            if (product.ProductId == 0)
            {
                _context.Products.Add(product);
            }
            else
            {
                Product dbEntry = _context.Products.Find(product.ProductId);
                if (dbEntry != null)
                {
                    dbEntry.Name = product.Name;
                    dbEntry.Description = product.Description;
                    dbEntry.Price = product.Price;
                    dbEntry.Category = product.Category;
                }
            }

            _context.SaveChanges();
        }
    }
}
