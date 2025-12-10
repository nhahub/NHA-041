using Microsoft.EntityFrameworkCore;
using test.Data;
using test.Interfaces;
using test.Models;

namespace test.Repository
{
    public class OrderRepository : IOrder
    {
        private readonly DepiContext _context;
        public OrderRepository(DepiContext depiContext) {
        
        _context= depiContext;
        }
        public bool AddOrder(Orders order)
        {
            _context.Orders.Add(order);
            return savechanges();
        }

        public async Task<List<Orders>> GetAllUserOrders(string UserID)
        {
            var orders = await _context.Orders.Where(o => o.UserId == UserID).ToListAsync();
            return orders;
        }

        public Orders GetOrderById(int id)
        {
            var order = _context.Orders.Find(id);
            return order;
        }

        public bool RemoveOrder(Orders order)
        {
            _context.Orders.Remove(order);
            return savechanges();
        }

        public bool savechanges()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
        public async Task<Orders> GetOrderFortransaction(int orderid)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderid);
            return order;
        }
    }
}
