using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;

        public TransactionsController(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        public IActionResult PostTransactions([FromBody] TransferDTO transferDTO)
        {
            using(var scope = new TransactionScope())
            {
                try
                {
                    if (transferDTO.amount == 0 || transferDTO.description == null || transferDTO.fromAccountNumber == null || transferDTO.toAccountNumber == null)
                    {
                        return StatusCode(403, "Hay datos nulos");
                    }

                    if (transferDTO.amount < 0)
                    {
                        return StatusCode(403, "El monto debe ser positivo");
                    }

                    if (transferDTO.fromAccountNumber.Equals(transferDTO.toAccountNumber))
                    {
                        return StatusCode(403, "La cuenta origen y la cuenta destino no pueden ser iguales");
                    }

                    if (!_accountRepository.ExistsByNumber(transferDTO.fromAccountNumber))
                    {
                        return StatusCode(403, "La cuenta origen no existe");
                    }

                    string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                    if (email == string.Empty)
                    {
                        return Forbid();
                    }

                    Client client = _clientRepository.FindByEmail(email);
                    if (!client.Accounts.Any(account => account.Number.ToUpper() == transferDTO.fromAccountNumber.ToUpper()))
                    {
                        return StatusCode(403, $"La cuenta {transferDTO.fromAccountNumber} no le pertenece");
                    }

                    if (!_accountRepository.ExistsByNumber(transferDTO.toAccountNumber))
                    {
                        return StatusCode(403, $"La cuenta destino no existe");
                    }

                    Account accountOrigen = _accountRepository.FindByNumber(transferDTO.fromAccountNumber);
                    Account accountDestino = _accountRepository.FindByNumber(transferDTO.toAccountNumber);

                    if (transferDTO.amount > accountOrigen.Balance)
                    {
                        return StatusCode(403, "No tiene suficientes fondos para realizar la transaccion");
                    }

                    Models.Transaction newTransactionOrigen = new Models.Transaction
                    {
                        AccountId = accountOrigen.Id,
                        Type = TransactionType.DEBIT,
                        Amount = transferDTO.amount,
                        Description = transferDTO.fromAccountNumber + transferDTO.description,
                        Date = DateTime.Now
                    };

                    Models.Transaction newTransactionDestino = new Models.Transaction
                    {
                        AccountId = accountDestino.Id,
                        Type = TransactionType.CREDIT,
                        Amount = transferDTO.amount,
                        Description = transferDTO.toAccountNumber + transferDTO.description,
                        Date = DateTime.Now
                    };

                    _transactionRepository.Save(newTransactionOrigen);
                    _transactionRepository.Save(newTransactionDestino);

                    accountOrigen.Balance -= transferDTO.amount;
                    accountDestino.Balance += transferDTO.amount;

                    _accountRepository.Save(accountOrigen);
                    _accountRepository.Save(accountDestino);

                    scope.Complete();
                    return Ok();

                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
        }
            

    }
}
