using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Utils;
using System.Net;

namespace HomeBankingMindHub.Services.Impl
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;
        public ClientService(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }
        public List<ClientDTO> getAllClients()
        {
            var clients = _clientRepository.GetAllClients();
            var listClientDTO = new List<ClientDTO>();

            foreach (Client client in clients)
            {
                var newClientDTO = new ClientDTO(client);
                listClientDTO.Add(newClientDTO);
            }

            return listClientDTO;
        }

        public Client getClientByEmail(string email)
        {
            return _clientRepository.FindByEmail(email);
        }
        public ClientDTO getClientDTOByEmail(string email)
        {
            Client client = getClientByEmail(email);

            if(client == null)
            {
                throw new Exception();
            }

            return new ClientDTO(client);

        }
        public Client getClientById(long id) 
        {
            return _clientRepository.FindById(id);
        }
        public ClientDTO getClientDTOById(long id) 
        {
            Client client = getClientById(id);

            if (client == null)
            {
                throw new Exception();
            }

            return new ClientDTO(client);
        }
        public bool clientExistsByEmail(string email)
        {
            return _clientRepository.ExistsByEmail(email);
        }
        public Client createNewClient(Client client)
        {
            string numAleatorio;
            do
            {
                numAleatorio = "VIN-" + NumberGenerator.GenerarNumero(0, 100000000);

            } while (_accountRepository.ExistsByNumber(numAleatorio));

            Client newClient = new Client(client, numAleatorio);

            _clientRepository.Save(newClient);

            return newClient;
        }

        public List<AccountDTO> getClientAccounts(string email)
        {
            Client client = _clientRepository.FindByEmail(email);

            if (client == null)
            {
                return null;
            }

            var listClientAccountsDTO = new List<AccountDTO>();

            foreach (Account account in client.Accounts)
            {
                var newAccountDTO = new AccountDTO(account);
                listClientAccountsDTO.Add(newAccountDTO);
            }

            return listClientAccountsDTO;
        }
        public List<CardDTO> getClientCards(string email)
        {
            Client client = _clientRepository.FindByEmail(email);

            if (client == null)
            {
                return null;
            }

            var listClientCardsDTO = new List<CardDTO>();

            foreach (Card card in client.Cards)
            {
                var newCardDTO = new CardDTO(card);
                listClientCardsDTO.Add(newCardDTO);
            }

            return listClientCardsDTO;
        }

        public Card createNewCard(Client client, CardCreateDTO cardCreateDTO)
        {
            int cvvAleatorio = NumberGenerator.GenerarNumero(0, 1000);
            string cvvFormateado = cvvAleatorio.ToString("D3");
            CardType cardType = (CardType)Enum.Parse(typeof(CardType), cardCreateDTO.type);
            CardColor cardColor = (CardColor)Enum.Parse(typeof(CardColor), cardCreateDTO.color);
            string cardNumberAleatorio;
            string cardNumberFormateado;
            do
            {
                cardNumberAleatorio = NumberGenerator.GenerarNumero(000000000000000, 10000000000000000).ToString("D16");
                cardNumberFormateado = cardNumberAleatorio.Insert(4, "-").Insert(9, "-").Insert(14, "-");

            } while (_cardRepository.ExistsByCardHolder(cardNumberFormateado));

            Card newCard = new Card(client, cvvFormateado, cardNumberFormateado, cardType, cardColor);

            _cardRepository.Save(newCard);

            return newCard;
        }
        public Account createNewAccount(Client client)
        {
            string numAleatorio;
            do
            {
                numAleatorio = "VIN-" + NumberGenerator.GenerarNumero(0, 100000000);

            } while (_accountRepository.ExistsByNumber(numAleatorio));

            Account newAccount = new Account(client, numAleatorio);

            _accountRepository.Save(newAccount);
            return newAccount;

        }
    }
}
