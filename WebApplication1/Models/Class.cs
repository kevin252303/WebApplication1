using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Class
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public byte[] PasswordHash {  get; set; }
        public byte[] PasswordSalt { get; set; }

    }

}
