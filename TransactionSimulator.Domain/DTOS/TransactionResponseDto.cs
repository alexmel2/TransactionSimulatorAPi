using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionSimulator.Domain.DTOS
{
    public record TransactionResponseDto(
    Guid TransactionId,
    string Status,
    DateTimeOffset SubmittedTimeUtc
);
}
