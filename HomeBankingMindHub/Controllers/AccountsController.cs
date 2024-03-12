using Microsoft.AspNetCore.Mvc;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Authorization;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]

    [ApiController]
    public class AccountsController : ControllerBase
    {

        private IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize(Policy="AdminOnly")]
        public IActionResult Get()
        {
            try
            {              
                return Ok(_accountService.getAllAccounts());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Get(long id)
        {
             
            try
            {
                var account = _accountService.getAccountById(id);
                if (account == null)
                {
                    return Forbid();
                }

                if (account.ClientId.ToString() != User.FindFirst("Id").Value)
                {
                    return Unauthorized();
                }

                var accountDTO = _accountService.getAccountDTOById(id);
  
                return Ok(accountDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
