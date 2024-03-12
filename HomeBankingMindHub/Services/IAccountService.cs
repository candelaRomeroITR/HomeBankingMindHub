using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IAccountService
    {
        public List<AccountDTO> getAllAccounts();
        public Account getAccountById(long id);
        public AccountDTO getAccountDTOById(long id);
        public Account getAccountByNumber(string number);
        public AccountDTO getAccountDTOByNumber(string number);
        bool accountExistsByNumber(string number);
        void saveAccount(Account account);
    }
}
