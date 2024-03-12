using HomeBankingMindHub.Models;

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HomeBankingMindHub.dtos

{
    public class ClientDTO
    {

        [JsonIgnore]

        public long Id { get; set; } //no devuelve Id por el JsonIgnore

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public ICollection<AccountDTO> Accounts { get; set; }
        public ICollection<ClientLoanDTO> Credits { get; set; }
        public ICollection<CardDTO> Cards { get; set; }
        public ClientDTO() { }
        public ClientDTO(Client client)
        {
            Id = client.Id;
            FirstName = client.FirstName;
            LastName = client.LastName;
            Email = client.Email;
            if (client.Accounts.Any())
            {
                Accounts = client.Accounts.Select(account => new AccountDTO(account)).ToList();
            } else
            {
                Accounts = new List<AccountDTO>();
            }
            if (client.ClientLoans.Any())
            {
                Credits = client.ClientLoans.Select(cl => new ClientLoanDTO(cl)).ToList();
            } else
            {
                Credits = new List<ClientLoanDTO>();
            }
            if (client.Cards.Any())
            {
                Cards = client.Cards.Select(card => new CardDTO(card)).ToList();
            }
            else
            {
                Cards = new List<CardDTO>();
            }
            
        }

    }
}