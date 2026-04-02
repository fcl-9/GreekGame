using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreekGame.API.Domain;

public sealed class City
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; init; }

    public int Population { get; set; }
    public int Food { get; set; }
    public decimal Money { get; set; }

    public DateTime LastUpdated { get; set; }
    public ICollection<Building> Buildings { get; init; } = new HashSet<Building>();
}
