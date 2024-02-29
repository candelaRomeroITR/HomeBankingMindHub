namespace HomeBankingMindHub.Models
{
    public class TransferDTO
    {
        public string fromAccountNumber { get; set; }
        public string toAccountNumber { get; set; }
        public double amount { get; set; }
        public string description { get; set; }
    }
}
