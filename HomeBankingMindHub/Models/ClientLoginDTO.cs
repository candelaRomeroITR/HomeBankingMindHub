using System.Text.Json.Serialization;

namespace HomeBankingMindHub.Models
{
    public class ClientLoginDTO
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
