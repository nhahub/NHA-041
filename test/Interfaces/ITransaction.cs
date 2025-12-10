using test.Models;

namespace test.Interfaces
{
    public interface ITransaction
    {
        public bool AddTransaction(Transactions transaction);
        public Transactions GetTransactionById(int id);
        public Task<List<Transactions>> GetAllUserTransactions(string UserID);
        public bool savechanges();
        public void beginTransaction();
        public void commitTransaction();
        public void rollbackTransaction();
        public Task savechangesAsync();
    }
}
