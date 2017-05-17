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
    }
}
