using System.Text.Json.Serialization;

namespace Domain.Models {
    public class Address {
        public int Id { get; set; }
        
        public string? Country { get; set; }
        
        public string? State { get; set; }
        
        public string? City { get; set; }
        
        public string? PostalCode { get; set; }
        
        public int CondominiumId {  get; set; }
        
        [JsonIgnore]
        public Condominium? Condominium { get; set; } = null!;
    }
}
