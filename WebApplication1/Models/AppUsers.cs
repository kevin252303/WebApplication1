using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Extentions;

namespace WebApplication1.Models
{
    public class AppUsers : IdentityUser<int>
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        public string? UserName { get; set; }
        public string password { get; set; }

        public byte[] PasswordHash {  get; set; }
        public byte[] PasswordSalt { get; set; }

        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string Gender { get; set; }
        public string LookingFor { get; set; }
        public string Introduction { get; set; }
        public string Interest {  get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<Photo> Photos { get; set; } = new ();
        public List<UserLike> LikedByUser { get; set; }
        public List<UserLike> LikedUsers {  get; set; }
        public List<Message> MessageSent {  get; set; }
        public List<Message> MessageReceived { get; set; }
        public string? instaUrl { get; set; }
        public string? twitterUrl { get; set; }



    }

}
