namespace HomeBankingMindHub.Models
{
    public class ClientLoan
    {
        public long Id { get; set; }
        public double Amount { get; set; }
        public string Payments { get; set; }
        public long ClientId { get; set; }
        public Client Client { get; set; }
        public long LoanId { get; set; }
        public Loan Loan { get; set; }
        public ClientLoan() { }
        public ClientLoan(LoanApplicationDTO loanApplicationDTO, Client client)
        {
            Amount = loanApplicationDTO.Amount + (loanApplicationDTO.Amount * 0.2);
            Payments = loanApplicationDTO.Payments;
            ClientId = client.Id;
            LoanId = loanApplicationDTO.LoanId;
        }
    }
}
