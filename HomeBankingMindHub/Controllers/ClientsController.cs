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
        public ClientsController(IClientRepository clientRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _cardRepository = cardRepository;
        }
     
        [HttpGet]
        [Authorize(Policy = "ClientOnly")]
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
        [Authorize(Policy = "ClientOnly")]
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

                //buscamos si ya existe el usuario
                //Client user = _clientRepository.FindByEmail(client.Email);

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

                // VERIFICAR QUE NO TENGA 3 CUENTAS CREADAS
                Client client = _clientRepository.FindByEmail(email);
                if (client.Accounts.Count == 3)
                {
                    return StatusCode(403, "No puedes tener mas de 3 cuentas");
                }

                //VERIFICAR NUM CUENTA NO EXISTA
                string numAleatorio;
                do
                {
                    numAleatorio = "VIN-" + NumberGenerator.GenerarNumero(0, 100000000);

                } while (client.Accounts.Any(ac => ac.Number == numAleatorio));

                //Agrego la cuenta al cliente
                Account newAccount = new Account
                {
                    ClientId = client.Id,
                    Number = numAleatorio,
                    CreationDate = DateTime.Now,
                    Balance = 0
                };

                _clientRepository.Save(client);
                return StatusCode(201, "Cuenta creada");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult PostCard([FromBody] Card card)
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

                //CardType typeCardEnum = (CardType)card.Type;
                CardType debito = (CardType)Enum.Parse(typeof(CardType), 1.ToString());
                CardType credito = (CardType)Enum.Parse(typeof(CardType), 0.ToString());

                if ((client.Cards.Count(c => c.Type == debito) == 3) || (client.Cards.Count(c => c.Type == credito) == 3))
                {
                    return StatusCode(403, $"Ya tiene el maximo de tarjetas de {card.Type}");
                }

                if (client.Cards.Any(c => c.Type == card.Type && c.Color == card.Color))
                {
                    return StatusCode(403, $"Ya tiene una tarjeta del tipo {card.Type} y del color {card.Color}");
                }

                int cvvAleatorio = NumberGenerator.GenerarNumero(0, 1000);

                string cardNumberAleatorio;
                do
                {
                    cardNumberAleatorio = NumberGenerator.GenerarNumero(0, 1000000).ToString(); //cambiar numero

                } while (_cardRepository.ExistsByCardHolder(cardNumberAleatorio));


                //Agrego la cuenta al cliente
                Card newCard = new Card
                {
                    ClientId = client.Id,
                    CardHolder = client.FirstName + client.LastName,
                    Type = card.Type,
                    Color = card.Color,
                    Number = cardNumberAleatorio,
                    Cvv = cvvAleatorio,
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(5),
                };
        
                _clientRepository.Save(client);
                return StatusCode(201, "Tarjeta agregada");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}