using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Xml.Linq;
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
        private readonly IMemoryCache _cache;

        public TransactionService(IApplicationDbContext context, IOptions<AppSettings> settings, ILogger<TransactionService> logger, IMemoryCache memory)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(IApplicationDbContext));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _cache = memory ?? throw new ArgumentNullException(nameof(IMemoryCache));
        }

        public async Task<IEnumerable<Region>> GetRegionsAsync()
        {
            try
            {
                
                if (! _cache.TryGetValue(DatabaseTable.Regions.ToString(), out List<Region> data ))
                {
                    return await _context.Regions.AsNoTracking().ToListAsync();
                }

                return data;
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
                Region? region = await GetRegion(regionId);
                Transaction transaction = BuildTransaction(transactionId, regionId, submittedTimeUtc, region);
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

        private Transaction BuildTransaction(Guid transactionId, int regionId, DateTime submittedTimeUtc, Region? region)
        {
            return new Transaction
            {
                TransactionId = transactionId,
                RegionId = regionId,
                SubmittedTime = submittedTimeUtc,
                Status = ValidateTransaction(submittedTimeUtc, region.TimeZoneId),
            };
        }

        private async Task<Region?> GetRegion(int regionId)
        {
            List<Region> regions = new();
            if (!_cache.TryGetValue(DatabaseTable.Regions.ToString(), out regions))
            {
                regions = await _context.Regions.AsNoTracking().ToListAsync();
            }
            var region = regions.FirstOrDefault(x => x.Id == regionId);
            if (region == null) throw new Exception("Region not found");
            return region;
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
