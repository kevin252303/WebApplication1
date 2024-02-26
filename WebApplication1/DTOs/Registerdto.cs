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

        public DateTime created {  get; set; }
        public DateTime lastActive { get; set; }
    }
}
