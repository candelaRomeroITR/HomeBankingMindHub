using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private IAccountService _accountService;
        private ITransactionService _transactionService;
        private IClientService _clientService;

        public TransactionsController(IAccountService accountService, ITransactionService transactionService, IClientService clientService)
        {
            _accountService = accountService;
            _transactionService = transactionService;
            _clientService = clientService;
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

                    if (!_accountService.accountExistsByNumber(transferDTO.fromAccountNumber))
                    {
                        return StatusCode(403, "La cuenta origen no existe");
                    }

                    string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                    if (email == string.Empty)
                    {
                        return Forbid();
                    }

                    Client client = _clientService.getClientByEmail(email);
                    if (!client.Accounts.Any(account => account.Number.ToUpper() == transferDTO.fromAccountNumber.ToUpper()))
                    {
                        return StatusCode(403, $"La cuenta {transferDTO.fromAccountNumber} no le pertenece");
                    }

                    if (!_accountService.accountExistsByNumber(transferDTO.toAccountNumber))
                    {
                        return StatusCode(403, $"La cuenta destino no existe");
                    }

                    Account accountOrigen = _accountService.getAccountByNumber(transferDTO.fromAccountNumber);
                    Account accountDestino = _accountService.getAccountByNumber(transferDTO.toAccountNumber);

                    if (transferDTO.amount > accountOrigen.Balance)
                    {
                        return StatusCode(403, "No tiene suficientes fondos para realizar la transaccion");
                    }

                    _transactionService.createTransaction(transferDTO);

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
