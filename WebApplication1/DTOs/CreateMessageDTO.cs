using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.DTOs
{
    public class CreateMessageDTO
    {
        public string RecipientUserName { get; set; }
        public string Content { get; set; }

        
    }
}
