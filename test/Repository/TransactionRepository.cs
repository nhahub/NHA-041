using Microsoft.EntityFrameworkCore;
using test.Data;
using test.Interfaces;
using test.Models;

namespace test.Repository
{
    public class TransactionRepository : ITransaction
    {
        private readonly DepiContext _context;
        public TransactionRepository(DepiContext depiContext)
        {
            _context = depiContext;
        }
        public bool AddTransaction(Transactions transaction)
        {
            _context.Transactions.Add(transaction);
            return savechanges();
        }

        public Task<List<Transactions>> GetAllUserTransactions(string UserID)
        {
            var transactions = _context.Transactions.Where(t => t.Order.UserId == UserID).ToListAsync();
            return transactions;
        }

        public Transactions GetTransactionById(int id)
        {
            var transaction = _context.Transactions.Find(id);
            return transaction;
        }

        public bool savechanges()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
        public void beginTransaction()
        {
            _context.Database.BeginTransaction();
        }
        public void commitTransaction()
        {
            _context.Database.CommitTransaction();
        }
        public void rollbackTransaction()
        {
            _context.Database.RollbackTransaction();
        }
        public async Task savechangesAsync()
        {
             await _context.SaveChangesAsync();
        }
    }
}
