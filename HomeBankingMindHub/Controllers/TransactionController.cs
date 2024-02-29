using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            try
            {
                //verifico que los parametros no esten vacios
                if (transferDTO.amount == 0 || transferDTO.description == null || transferDTO.fromAccountNumber == null || transferDTO.toAccountNumber == null)
                {
                    return StatusCode(403, "Hay datos nulos");
                }

                //verifico que los numeros de cuenta no sean iguales
                if (transferDTO.fromAccountNumber.Equals(transferDTO.toAccountNumber))
                {
                    return StatusCode(403, "La cuenta origen y la cuenta destino no pueden ser iguales");
                }

                //verifico que exista la cuenta de origen
                if (!_accountRepository.ExistsByNumber(transferDTO.fromAccountNumber))
                {
                    return StatusCode(403, "La cuenta origen no existe");
                }

                //verifico que la cuenta origen sea la del cliente autenticado
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

                //verifico que la cuenta destino exista
                if (!_accountRepository.ExistsByNumber(transferDTO.toAccountNumber))
                {
                    return StatusCode(403, $"La cuenta destino no existe");
                }

                //verifico cuenta origen tenga el monto a transferir disponible
                Account accountOrigen = _accountRepository.FindByNumber(transferDTO.fromAccountNumber);
                Account accountDestino = _accountRepository.FindByNumber(transferDTO.toAccountNumber);

                if (transferDTO.amount > accountOrigen.Balance)
                {
                    return StatusCode(403, "No tiene suficientes fondos para realizar la transaccion");
                }

                //crear transacciones 
                Transaction newTransactionOrigen = new Transaction
                {
                    AccountId = accountOrigen.Id,
                    Type = TransactionType.DEBIT,
                    Amount = transferDTO.amount,
                    Description = transferDTO.fromAccountNumber + transferDTO.description,
                    Date = DateTime.Now
                };

                Transaction newTransactionDestino = new Transaction
                {
                    AccountId = accountDestino.Id,
                    Type = TransactionType.CREDIT,
                    Amount = transferDTO.amount,
                    Description = transferDTO.toAccountNumber + transferDTO.description,
                    Date = DateTime.Now
                };

                _transactionRepository.Save(newTransactionOrigen);
                _transactionRepository.Save(newTransactionDestino);
                return StatusCode(201, "Transaccion creada");
                
            } 
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
