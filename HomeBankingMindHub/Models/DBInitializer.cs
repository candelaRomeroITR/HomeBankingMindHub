namespace HomeBankingMindHub.Models
{
    public class DBInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client { Email = "vcoronado@gmail.com", FirstName="Victor", LastName="Coronado", Password="123456"},
                    new Client { Email = "cromero@gmail.com", FirstName="Candela", LastName="Romero", Password="123456"},
                    new Client { Email = "pllinas@gmail.com", FirstName="Paula", LastName="Llinas", Password="123456"},
                    new Client { Email = "jgiorgetti@gmail.com", FirstName="Julieta", LastName="Giorgetti", Password="123456"}
                };

                context.Clients.AddRange(clients);

                //guardamos
                context.SaveChanges();
            }

            if (!context.Account.Any())
            {
                var accountVictor = context.Clients.FirstOrDefault(cliente => cliente.Email == "vcoronado@gmail.com");
                var accountCandela = context.Clients.FirstOrDefault(cliente => cliente.Email == "cromero@gmail.com");
                var accountPaula = context.Clients.FirstOrDefault(cliente => cliente.Email == "pllinas@gmail.com");
                var accountJulieta = context.Clients.FirstOrDefault(cliente => cliente.Email == "jgiorgetti@gmail.com");

                if (accountVictor != null && accountCandela != null && accountPaula != null && accountJulieta != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = accountVictor.Id, CreationDate = DateTime.Now, Number = "VIN001", Balance = 0 },
                        new Account {ClientId = accountVictor.Id, CreationDate = DateTime.Now.AddDays(-5), Number = "VIN002", Balance = 1000 },
                        new Account {ClientId = accountCandela.Id, CreationDate = DateTime.Now.AddDays(-10), Number = "VIN003", Balance = 2000 },
                        new Account {ClientId = accountPaula.Id, CreationDate = DateTime.Now.AddDays(-1), Number = "VIN004", Balance = 2500 },
                        new Account {ClientId = accountJulieta.Id, CreationDate = DateTime.Now.AddDays(-30), Number = "VIN004", Balance = 0 },
                        new Account {ClientId = accountJulieta.Id, CreationDate = DateTime.Now.AddDays(-1), Number = "VIN005", Balance = 4000 },
                        new Account {ClientId = accountCandela.Id, CreationDate = DateTime.Now.AddDays(+4), Number = "VIN006", Balance = 5500 }

                    };
                    foreach (Account account in accounts)
                    {
                        context.Account.AddRange(accounts);
                    }
                    context.SaveChanges();

                }
            }
        }
    }
}
