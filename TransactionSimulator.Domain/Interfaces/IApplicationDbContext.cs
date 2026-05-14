using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionSimulator.Domain.Entities;

namespace TransactionSimulator.Domain.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Transaction> Transactions { get; set; }
        DbSet<Region> Regions { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
