using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GreekGame.API.Domain;

public class Building
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public Guid CityId { get; set; }
    public BuildingType Type { get; set; }
    public byte Level { get; set; }
    public DateTime LastUpdated { get; set; }
    public virtual City City { get; set; }
}
