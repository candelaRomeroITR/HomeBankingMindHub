using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using HomeBankingMindHub.Utils;
using System.Drawing.Text;

namespace HomeBankingMindHub.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {

        private IClientRepository _clientRepository;
        private ICardRepository _cardRepository;
        private IAccountRepository _accountRepository;
        public ClientsController(IClientRepository clientRepository, ICardRepository cardRepository, IAccountRepository accountRepository)
        {
            _clientRepository = clientRepository;
            _cardRepository = cardRepository;
            _accountRepository = accountRepository;
        }
     
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();
                var clientsDTO = new List<ClientDTO>();

                foreach (Client client in clients)
                {
                    var newClientDTO = new ClientDTO
                    {
                        Id = client.Id,
                        Email = client.Email,
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        Accounts = client.Accounts.Select(ac => new AccountDTO
                        {

                            Id = ac.Id,
                            Balance = ac.Balance,
                            CreationDate = ac.CreationDate,
                            Number = ac.Number

                        }).ToList(),

                        Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                        {

                            Id = cl.Id,
                            LoanId = cl.LoanId,
                            Name = cl.Loan.Name,
                            Amount = cl.Amount,
                            Payments = int.Parse(cl.Payments)

                        }).ToList(),

                        Cards = client.Cards.Select(c => new CardDTO
                        {

                            Id = c.Id,
                            CardHolder = c.CardHolder,
                            Color = c.Color.ToString(),
                            Cvv = c.Cvv,
                            FromDate = c.FromDate,
                            Number = c.Number,
                            ThruDate = c.ThruDate,
                            Type = c.Type.ToString()

                        }).ToList()
                    };
                    clientsDTO.Add(newClientDTO);
                }
                return Ok(clientsDTO);
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

                var client = _clientRepository.FindById(id);

                if (client == null)
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO
                {

                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO

                    {

                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number

                    }).ToList(),

                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO

                    {

                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)

                    }).ToList(),

                    Cards = client.Cards.Select(c => new CardDTO
                    {

                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString()

                    }).ToList()

                };

                return Ok(clientDTO);

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

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString(),
                    }).ToList()
                };

                return Ok(clientDTO);
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


                if (_clientRepository.ExistsByEmail(client.Email))
                {
                    return StatusCode(403, "Email está en uso");
                }

                Client newClient = new Client
                {
                    Email = client.Email,
                    Password = client.Password,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                };

                _clientRepository.Save(newClient);
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

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                };

                return Ok(clientDTO);
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
                Client client = _clientRepository.FindByEmail(email);
                if (client.Accounts.Count == 3)
                {
                    return StatusCode(403, "No puedes tener mas de 3 cuentas");
                }

                //verifico num de cuenta no exista
                string numAleatorio;
                do
                {
                    numAleatorio = "VIN-" + NumberGenerator.GenerarNumero(0, 100000000);

                } while (_accountRepository.ExistsByNumber(numAleatorio));

                //agrego la cuenta al cliente
                Account newAccount = new Account
                {
                    ClientId = client.Id,
                    Number = numAleatorio,
                    CreationDate = DateTime.Now,
                    Balance = 0
                };

                _accountRepository.Save(newAccount);
                return StatusCode(201, "Cuenta creada");
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

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString(),
                    }).ToList()
                };

                return Ok(clientDTO);
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
                Client client = _clientRepository.FindByEmail(email);
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

                int cvvAleatorio = NumberGenerator.GenerarNumero(0, 1000);
                string cvvFormateado = cvvAleatorio.ToString("D3");

                string cardNumberAleatorio;
                do
                {
                    cardNumberAleatorio = NumberGenerator.GenerarNumero(000000000000000, 10000000000000000).ToString("D16"); 

                } while (_cardRepository.ExistsByCardHolder(cardNumberAleatorio));
                string cardNumberFormateado = cardNumberAleatorio.Insert(4, "-").Insert(9, "-").Insert(14, "-");

                //Agrego la cuenta al cliente
                Card newCard = new Card
                {
                    ClientId = client.Id,
                    CardHolder = client.FirstName + client.LastName,
                    Type = cardType,
                    Color = cardColor,
                    Number = cardNumberFormateado,
                    Cvv = int.Parse(cvvFormateado),
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(5),
                };
        
                _cardRepository.Save(newCard);
                return StatusCode(201, "Tarjeta agregada");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}