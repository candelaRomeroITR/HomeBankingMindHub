namespace HomeBankingMindHub.Models

{
    public class Transaction
    {
        public long Id { get; set; }
        public TransactionType Type { get; set; } 
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public Account Account { get; set; }
        public long AccountId { get; set; }
        public Transaction() { }
        public Transaction(Account account, TransferDTO transferDTO, TransactionType type)
        {
            AccountId = account.Id;
            Type = type;
            Amount = transferDTO.amount;
            Description = transferDTO.fromAccountNumber + transferDTO.description;
            Date = DateTime.Now;
        }
        public Transaction(Account account, LoanApplicationDTO loanApplicationDTO, Loan loan)
        {
            AccountId = account.Id;
            Type = TransactionType.CREDIT;
            Amount = loanApplicationDTO.Amount;
            Description = $"{loan.Name} loan approved";
            Date = DateTime.Now;
        }
 
    }
}
