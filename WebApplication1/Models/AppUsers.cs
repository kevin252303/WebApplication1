using System.ComponentModel.DataAnnotations;
using WebApplication1.Extentions;

namespace WebApplication1.Models
{
    public class AppUsers
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
        public string password { get; set; }

        public byte[] PasswordHash {  get; set; }
        public byte[] PasswordSalt { get; set; }
        //public DateOnly DateOfBirth { get; set; }
        public string KnownAs { get; set; }
       // public DateTime Created { get; set; } = DateTime.UtcNow;
        //public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string Gender { get; set; }
        public string LookingFor { get; set; }
        public string Introduction { get; set; }
        public string Interest {  get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<Photo> Photos { get; set; } = new List<Photo>();

        public int GetAge()
        {
            //return DateOfBirth.CalculateAge();
            return 0;
        }



    }

}
