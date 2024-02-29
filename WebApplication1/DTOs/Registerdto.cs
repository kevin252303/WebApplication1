using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs
{
    public class Registerdto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(15,MinimumLength =4)]
        public string Password { get; set; }
        [Required]public DateOnly? DateOfBirth { get; set; }
        [Required]public string KnownAs { get; set; }
        [Required]public string Gender { get; set; }
        [Required]public string City { get; set; }
        [Required]public string Country { get; set; }
    }
}
