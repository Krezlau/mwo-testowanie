using System.ComponentModel.DataAnnotations;

namespace mwo_testowanie.Models.DTOs;

public class ClientCreateDTO
{
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string Name { get; set; }
    
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string Surname { get; set; }
    
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; }
    
}