using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionSimulator.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        public Guid TransactionId { get; set; }
        public int RegionId { get; set; }
        public Region? Region { get; set; }
        public DateTime SubmittedTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}
