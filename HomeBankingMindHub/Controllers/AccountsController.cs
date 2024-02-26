using HomeBankingMindHub.dtos;

using HomeBankingMindHub.Models;

using HomeBankingMindHub.Repositories;

using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Security.Claims;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]

    [ApiController]
    public class AccountsController : ControllerBase
    {

        private IAccountRepository _accountRepository;

        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]

        public IActionResult Get()

        {

            try

            {

                var accounts = _accountRepository.GetAllAccounts();



                var accountDTO = new List<AccountDTO>();



                foreach (Account account in accounts)

                {

                    var newAccountDTO = new AccountDTO

                    {

                        Id = account.Id,

                        Number = account.Number,

                        CreationDate = account.CreationDate,

                        Balance = account.Balance,

                        Transactions = account.Transactions.Select(tr => new TransactionDTO

                        {

                            Id = tr.Id,

                            Type = tr.Type,

                            Amount = tr.Amount,

                            Description = tr.Description,

                            Date = tr.Date,

                        }).ToList()

                    };



                    accountDTO.Add(newAccountDTO);

                }





                return Ok(accountDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }

        [HttpGet("{id}")]

        public IActionResult Get(long id)

        {
              
            try

            {

                var account = _accountRepository.FindById(id);

                if (account.ClientId.ToString() != User.FindFirst("Id").Value)
                {
                    return Unauthorized();
                }

                if (account == null)

                {

                    return Forbid();

                }



                var accountDTO = new AccountDTO

                {

                    Id = account.Id,

                    Number = account.Number,

                    CreationDate = account.CreationDate,

                    Balance = account.Balance,

                    Transactions = account.Transactions.Select(ac => new TransactionDTO

                    {

                        Id = ac.Id,

                        Type = ac.Type,

                        Amount = ac.Amount,

                        Description = ac.Description,

                        Date = ac.Date,

                    }).ToList()

                };



                return Ok(accountDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }
    }
}
