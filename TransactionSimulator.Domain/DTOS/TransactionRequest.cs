using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionSimulator.Domain.DTOS
{
    public record TransactionRequest(
    Guid TransactionId,
    int RegionId,

    DateTime SubmittedTimeUtc
);
}
