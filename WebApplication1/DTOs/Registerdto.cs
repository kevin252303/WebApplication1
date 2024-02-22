using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs
{
    public class Registerdto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
