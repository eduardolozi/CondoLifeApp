using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Filters;

public class VotingFilter
{
    [Required (ErrorMessage = "CondominiumId deve ser especificado")] 
    public int CondominiumId { get; set; }
    public DateTime? BaseDate { get; set; }
    public bool IsOpened { get; set; }
}