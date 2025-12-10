using test.Models;

namespace test.Interfaces
{
    public interface IOrder
    {
        public bool AddOrder(Orders order);
        public bool RemoveOrder(Orders order);
        public Orders GetOrderById(int id);
        public  Task<List<Orders>> GetAllUserOrders(string UserID);
        public bool savechanges();
        public Task<Orders> GetOrderFortransaction(int orderid);
    }
}
