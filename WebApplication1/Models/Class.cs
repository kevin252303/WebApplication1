using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Class
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Password { get; set; }

    }

}
