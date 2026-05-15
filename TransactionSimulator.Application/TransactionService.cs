using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TransactionSimulator.Domain.Config;
using TransactionSimulator.Domain.Entities;
using TransactionSimulator.Domain.Enums;
using TransactionSimulator.Domain.Interfaces;
namespace Application
{

    public class TransactionService : ITransactionService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<TransactionService> _logger;
        private readonly AppSettings _settings; 

        public TransactionService(IApplicationDbContext context, IOptions<AppSettings> settings, ILogger<TransactionService> logger)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<IEnumerable<Region>> GetRegionsAsync()
        {
            try
            {
                return await _context.Regions.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Get Regions");
                return Enumerable.Empty<Region>();
            }
        }   

        public async Task<string> ProcessTransactionAsync(Guid transactionId, int regionId, DateTime submittedTimeUtc)
        {
            try
            {
                var region = await _context.Regions.AsNoTracking().FirstOrDefaultAsync(x=> x.Id == regionId);
                if (region == null) throw new Exception("Region not found");

                var transaction = new Transaction
                {
                    TransactionId = transactionId,
                    RegionId = regionId,
                    SubmittedTime = submittedTimeUtc,
                    Status = ValidateTransaction(submittedTimeUtc, region.TimeZoneId),
                };
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                return transaction.Status;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"An error occurred while processing Transaction {transactionId}");
                return TransactionStatus.Rejected.ToString();
            }
        }

        public async Task<IEnumerable<Transaction>> GetApprovedTransactionsAsync(int pageNumber, int pageSize)
        {

            try
            {
                if (pageNumber < _settings.PagingConfig.DefaultPageNumber) pageNumber = _settings.PagingConfig.DefaultPageNumber;
                if (pageSize > _settings.PagingConfig.MaxPageSize)       pageSize = _settings.PagingConfig.MaxPageSize;

                return await _context.Transactions
                    .Include(t => t.Region)
                    .Where(t => t.Status == TransactionStatus.Approved.ToString())
                    .OrderByDescending(t => t.CreatedAtUtc)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Get Approved Transactions");
                return Enumerable.Empty<Transaction>();
            }
        }

        public string ValidateTransaction(DateTime SubmittedTimeUtc, string timeZoneId)
        {
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(SubmittedTimeUtc, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
            var open = TimeSpan.Parse(_settings.BankPolicy.DefaultOpeningTime);
            var close = TimeSpan.Parse(_settings.BankPolicy.DefaultClosingTime);
            return (localTime.TimeOfDay > open && localTime.TimeOfDay < close) ? TransactionStatus.Approved.ToString() : TransactionStatus.Rejected.ToString();
        }
    }
}
