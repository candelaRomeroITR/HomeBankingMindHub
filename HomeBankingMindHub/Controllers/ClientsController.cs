using HomeBankingMindHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeBankingMindHub.Utils;
using HomeBankingMindHub.Services;

namespace HomeBankingMindHub.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {

        private IClientService _clientService;
        private IAccountService _accountService;
        public ClientsController(IClientService clientService, IAccountService accountService)
        {
            _clientService = clientService;
            _accountService = accountService;
        }
     
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                return Ok(_clientService.getAllClients());
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get(long id)
        {
            try
            {
                return Ok(_clientService.getClientDTOById(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                } 

                return Ok(_clientService.getClientDTOByEmail(email));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Client client)
        {
            try
            {
                //validamos datos antes
                if (String.IsNullOrEmpty(client.Email))
                {
                    return StatusCode(403, "email inválido");
                }
                if (String.IsNullOrEmpty(client.Password))
                {
                    return StatusCode(403, "contraseña inválida");
                }
                if (String.IsNullOrEmpty(client.FirstName))
                {
                    return StatusCode(403, "Nombre inválido");
                }
                if (String.IsNullOrEmpty(client.LastName))
                {
                    return StatusCode(403, "Apellido inválido");
                }


                if (_clientService.clientExistsByEmail(client.Email))
                {
                    return StatusCode(403, "Email está en uso");
                }

                string numAleatorio;
                do
                {
                    numAleatorio = "VIN-" + NumberGenerator.GenerarNumero(0, 100000000);

                } while (_accountService.accountExistsByNumber(numAleatorio));

                Client newClient = _clientService.createNewClient(client);
                
                return Created("", newClient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrentAccounts()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                } 

                return Ok(_clientService.getClientAccounts(email));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult PostAccount() 
        {
            try
            {
                //obtener info cliente autenticado
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                //verifico que no tenga 3 cuentas creadas
                Client client = _clientService.getClientByEmail(email);
                if (client.Accounts.Count == 3)
                {
                    return StatusCode(403, "No puedes tener mas de 3 cuentas");
                }

                Account newAccount = _clientService.createNewAccount(client);

                return StatusCode(201, newAccount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrentCards()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }
  
                return Ok(_clientService.getClientCards(email));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult PostCard([FromBody] CardCreateDTO cardCreateDTO)
        {
            try
            {
                //obtener info cliente autenticado
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                // VERIFICAR MAX TARJETAS - COLORES
                Client client = _clientService.getClientByEmail(email);
                if (client.Cards.Count == 6)
                {
                    return StatusCode(403, "No puedes tener mas de 6 tarjetas");
                }

                CardType cardType = (CardType)Enum.Parse(typeof(CardType), cardCreateDTO.type);
                CardColor cardColor = (CardColor)Enum.Parse(typeof(CardColor), cardCreateDTO.color);

                if (client.Cards.Count(c => c.Type == cardType) == 3)
                {
                    return StatusCode(403, $"Ya tiene el maximo de tarjetas de {cardCreateDTO.type}");
                }

                if (client.Cards.Where(c => c.Type == cardType).Any(c => c.Color == cardColor))
                {
                    return StatusCode(403, $"Ya tiene una tarjeta del tipo {cardCreateDTO.type} y del color {cardCreateDTO.color}");
                }

                Card newCard = _clientService.createNewCard(client, cardCreateDTO);

                return StatusCode(201, newCard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}