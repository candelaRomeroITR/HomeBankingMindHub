namespace HomeBankingMindHub.Models
{
    public class Client
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<Account> Accounts { get; set; }
        public ICollection<ClientLoan> ClientLoans { get; set; }
        public ICollection<Card> Cards { get; set; }
        public Client() { }
        public Client(Client client, string numAleatorio)
        {
            FirstName = client.FirstName;
            LastName = client.LastName;
            Email = client.Email;
            Password = client.Password;
            Accounts = new List<Account>
                    {
                        new Account
                        {
                            ClientId = client.Id,
                            Number = numAleatorio,
                            CreationDate = DateTime.Now,
                            Balance = 0
                        }
                    };

        }
    }
}
