using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Tilo.Models
{
    public class EFOrdersRepository: IOrdersRepository
    {
        private ApplicationDbContext context;

        public EFOrdersRepository(ApplicationDbContext ctx)
        {
             context = ctx;
        }
        public IEnumerable<Order> Orders => context.Orders
            .Include(o => o.Lines).ThenInclude(l => l.Product).ThenInclude(p => p.Category);

        public Order GetOrder(long key) => context.Orders
            .Include(o => o.Lines).ThenInclude(l => l.Product).ThenInclude(p => p.Category).First(o => o.Id == key);

        public void AddOrder(Order order)
        {
            order.dateTime = DateTime.Now;
            var entry = context.Orders.Add(order);
            context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            context.Orders.Update(order);
            context.SaveChanges();
        }

        public void DeleteOrder(Order order)
        {
            context.Orders.Remove(order);
            context.SaveChanges();
        }
    }
}
