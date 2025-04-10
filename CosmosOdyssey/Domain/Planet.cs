using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class Planet : BaseEntity
{
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    [InverseProperty("From")]
    public ICollection<RouteInfo>? LegsFrom { get; set; }
    
    [InverseProperty("To")]
    public ICollection<RouteInfo>? LegsTo { get; set; }
}