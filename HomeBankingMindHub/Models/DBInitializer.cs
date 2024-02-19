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

            if (!context.Transactions.Any())
            {
                var account1 = context.Account.FirstOrDefault(cliente => cliente.Number == "VIN001");
                var account2 = context.Account.FirstOrDefault(cliente => cliente.Number == "VIN002");
                var account3 = context.Account.FirstOrDefault(cliente => cliente.Number == "VIN003");
                var account4 = context.Account.FirstOrDefault(cliente => cliente.Number == "VIN004");
                var account5 = context.Account.FirstOrDefault(cliente => cliente.Number == "VIN005");
                var account6 = context.Account.FirstOrDefault(cliente => cliente.Number == "VIN006");

                if (account1 != null && account2 != null && account3 != null && account4 != null && account5 != null && account6 != null)
                {

                    var transactions = new Transaction[]
                    {
                        new Transaction { AccountId= account1.Id, Amount = 10000, Date= DateTime.Now.AddHours(-5), Description = "Transferencia recibida", Type = TransactionType.CREDIT },

                        new Transaction { AccountId= account1.Id, Amount = -2000, Date= DateTime.Now.AddHours(-6), Description = "Compra en tienda mercado libre", Type = TransactionType.DEBIT },

                        new Transaction { AccountId= account1.Id, Amount = -3000, Date= DateTime.Now.AddHours(-7), Description = "Compra en tienda xxxx", Type = TransactionType.DEBIT },

                        new Transaction { AccountId= account2.Id, Amount = 30000, Date= DateTime.Now.AddHours(-10), Description = "Transferencia recibida", Type = TransactionType.CREDIT },

                        new Transaction { AccountId= account2.Id, Amount = -10000, Date= DateTime.Now.AddHours(-9), Description = "Compra en tienda xxxx", Type = TransactionType.DEBIT },

                        new Transaction { AccountId= account3.Id, Amount = 7000, Date= DateTime.Now.AddHours(-2), Description = "Transferencia recibida", Type = TransactionType.CREDIT },

                        new Transaction { AccountId= account3.Id, Amount = -3000, Date= DateTime.Now, Description = "Compra en tienda xxxx", Type = TransactionType.DEBIT },

                        new Transaction { AccountId= account4.Id, Amount = 15000, Date= DateTime.Now.AddHours(-4), Description = "Transferencia recibida", Type = TransactionType.DEBIT },

                        new Transaction { AccountId= account4.Id, Amount = -4000, Date= DateTime.Now.AddHours(-2), Description = "Compra en tienda xxxx", Type = TransactionType.DEBIT },

                        new Transaction { AccountId= account5.Id, Amount = 10000, Date= DateTime.Now.AddHours(-2), Description = "Transferencia recibida", Type = TransactionType.CREDIT },

                        new Transaction { AccountId= account5.Id, Amount = -5000, Date= DateTime.Now.AddHours(-1), Description = "Compra en tienda xxxx", Type = TransactionType.DEBIT },

                        new Transaction { AccountId= account6.Id, Amount = 11000, Date= DateTime.Now, Description = "Compra en tienda xxxx", Type = TransactionType.DEBIT },

                    };

                    foreach (Transaction transaction in transactions)

                    {
                        context.Transactions.Add(transaction);
                    }

                    context.SaveChanges();
                }

            }
        }
    }
}
