namespace TransactionSimulator.Api.Validators
{
    using FluentValidation;
    using TransactionSimulator.Domain.DTOS;

    public class TransactionRequestValidator : AbstractValidator<TransactionRequest>
    {
        public TransactionRequestValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .WithMessage("Transaction ID is missing or invalid.");

            RuleFor(x => x.SubmittedTimeUtc)
                .NotEmpty()
                .NotEqual(default(DateTime))
                .WithMessage("A valid UTC timestamp is required.");

            RuleFor(x => x.RegionId)
                .GreaterThan(0)
                .WithMessage("Please select a valid Region.");
        }
    }
}
