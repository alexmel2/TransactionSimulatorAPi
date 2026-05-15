using Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TransactionSimulator.Domain.Entities;
using TransactionSimulator.Domain.Enums;
using TransactionSimulator.Domain.Interfaces;
namespace TransactionSimulator.Infrastructure.Caching
{

    public class RegionCacheService : BackgroundService
    {
        private readonly ILogger<RegionCacheService> _logger;
        private readonly IMemoryCache _cache;
        private readonly IServiceProvider _serviceProvider;
        private readonly string CacheKey ;

        public RegionCacheService(IMemoryCache cache, IServiceProvider serviceProvider, ILogger<RegionCacheService> logger)
        {
             CacheKey = DatabaseTable.Regions.ToString();
            _cache = cache;
            _serviceProvider = serviceProvider;
            _logger = logger  ?? throw new ArgumentNullException(nameof(logger)); ;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await RefreshCacheSafeAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                TimeSpan delay = CalculateDelayUntilMidnight();

                await Task.Delay(delay, stoppingToken);

                await RefreshCacheSafeAsync();
            }
        }
        private TimeSpan CalculateDelayUntilMidnight()
        {
            var now = DateTime.Now;
            var nextMidnight = now.Date.AddDays(1); 
            return nextMidnight - now;
        }

        private async Task RefreshCacheSafeAsync()
        {
            try
            {
                var regions = await FetchRegionsFromDatabaseAsync();
                UpdateCache(regions);

                _logger.LogInformation($"[Cache] Regions reloaded successfully at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"[Cache Error] Failed to reload Regions");
            }
        }

        private async Task<List<Region>> FetchRegionsFromDatabaseAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                return await dbContext.Regions.AsNoTracking().ToListAsync();
            }
        }

       
        private void UpdateCache(List<Region> regions)
        {
            _cache.Set(CacheKey, regions);
        }
    }
}