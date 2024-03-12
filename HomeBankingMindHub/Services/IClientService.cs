using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {
        public List<ClientDTO> getAllClients();
        public ClientDTO getClientDTOByEmail(string email);
        public Client getClientByEmail(string email);
        public Client getClientById(long id);
        public ClientDTO getClientDTOById(long id);
        public bool clientExistsByEmail(string email);
        public Client createNewClient(Client client);
        public List<AccountDTO> getClientAccounts(string email);
        public List<CardDTO> getClientCards(string email);
        public Card createNewCard(Client client, CardCreateDTO cardCreateDTO);
        public Account createNewAccount(Client client);
    }
}
