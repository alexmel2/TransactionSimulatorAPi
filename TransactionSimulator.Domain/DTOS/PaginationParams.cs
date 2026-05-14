using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionSimulator.Domain.DTOS
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; init; } = 1;
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            init => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
