using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Company
{
    public Guid Id { get; set; }
    
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    public ICollection<Provider>? Providers { get; set; }
}