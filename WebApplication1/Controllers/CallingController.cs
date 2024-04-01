using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.SignalR;

namespace WebApplication1.Controllers
{
    public class CallingController : BaseApiController
    {
        private readonly IHubContext<CallHub> _hubContext;

        public CallingController(IHubContext<CallHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("receive")]
        public IActionResult ReceiveCall([FromBody] string userId)
        {
            // Logic to handle incoming call
            // Notify the user of the incoming call using SignalR
            _hubContext.Clients.User(userId).SendAsync("ReceiveIncomingCall", "Caller Name");

            return Ok("Incoming call notification sent successfully");
        }
    }
}
