using TransactionSimulator.Domain.Entities;

namespace TransactionSimulator.Domain.Interfaces
{
    public interface ITransactionService
    {
        Task<string> ProcessTransactionAsync(Guid transactionId, int regionId, DateTime submittedTimeUtc);
        Task<IEnumerable<Transaction>> GetApprovedTransactionsAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Region>> GetRegionsAsync();
    }
}
