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
        }
    }
}
