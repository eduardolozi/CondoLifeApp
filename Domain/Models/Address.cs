using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models {
    [NotMapped]
    public class Address {
        public int Id { get; set; }
        public required string Country { get; set; }
        public required string State { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public int CondominiumId {  get; set; }
        public Condominium Condominium { get; set; } = null!;
    }
}
