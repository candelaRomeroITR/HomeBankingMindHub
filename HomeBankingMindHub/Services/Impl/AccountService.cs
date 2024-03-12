using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Impl
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

         public List<AccountDTO> getAllAccounts()
         {
            var accounts = _accountRepository.GetAllAccounts();
            var listAccountDTO = new List<AccountDTO>();
            foreach (Account account in accounts) 
            {
                var newAccountDTO = new AccountDTO(account);
                listAccountDTO.Add(newAccountDTO);
            }
            return listAccountDTO;
         }
         
         public Account getAccountById(long id)
        {
            return _accountRepository.FindById(id);
        }

         public AccountDTO getAccountDTOById(long id) 
         {
            Account account = getAccountById(id);
            if (account == null)
            {
                return null;
            }
            return new AccountDTO(account);
         }
        public Account getAccountByNumber(string number)
        {
            return _accountRepository.FindByNumber(number);
        }
        public AccountDTO getAccountDTOByNumber(string number)
        {
            Account account = getAccountByNumber(number);
            if(account == null)
            {
                return null;
            }
            return new AccountDTO(account);
        }

        public bool accountExistsByNumber(string number)
        {
            return _accountRepository.ExistsByNumber(number);
        }
        public void saveAccount(Account account)
        {
            _accountRepository.Save(account);
        }
    }
}
