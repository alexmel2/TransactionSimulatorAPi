using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionSimulator.Domain.DTOS;
using TransactionSimulator.Domain.Entities;
using TransactionSimulator.Domain.Interfaces; 

namespace ShvaSimulator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("GetApprovedTransactions")]
        public async Task<ActionResult<IEnumerable<TransactionResponseDto>>> GetApprovedTransactions([FromQuery] int pageSize = 10,int page= 1)
        {
            var history = await _transactionService.GetApprovedTransactionsAsync(page,pageSize);
            return Ok(history);
        }
        [HttpGet("regions")]
        public async Task<ActionResult<IEnumerable<Region>>> GetRegions()
        {
            var regions = await _transactionService.GetRegionsAsync();
            return Ok(regions);
        }
       [HttpPost("CreateTransaction")]
        public async Task<ActionResult<TransactionResponseDto>> CreateTransaction([FromBody] TransactionRequest request)
        {
            var result = await _transactionService.ProcessTransactionAsync(request.TransactionId, request.RegionId, request.SubmittedTimeUtc);
            return new TransactionResponseDto(request.TransactionId, result, request.SubmittedTimeUtc);
        }
    }
}