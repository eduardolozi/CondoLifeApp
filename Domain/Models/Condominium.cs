using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models {
    [NotMapped]
    public class Condominium {
        public int Id { get; set; }
        public required string Name { get; set; }
        public Address Address { get; set; } = null!;
        public List<User> Users { get; set; } = [];
    }
}
