using Microsoft.VisualBasic;

namespace HomeBankingMindHub.Models
{
    public class Card
    {
        public long Id { get; set; }
        public string CardHolder { get; set; }
        public CardType Type { get; set; }
        public CardColor Color { get; set; }
        public string Number { get; set; }
        public int Cvv { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ThruDate { get; set; }
        public Client Client { get; set; }
        public long ClientId { get; set; }
        public Card() { }
        public Card(Client client, string cvvFormateado, string cardNumberFormateado, CardType cardType, CardColor cardColor)
        {
            ClientId = client.Id;
            CardHolder = $"{client.FirstName} {client.LastName}";
            Type = cardType;
            Color = cardColor;
            Number = cardNumberFormateado;
            Cvv = int.Parse(cvvFormateado);
            FromDate = DateTime.Now;
            ThruDate = DateTime.Now.AddYears(5);
        }
    }
}
