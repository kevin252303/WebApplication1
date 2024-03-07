namespace WebApplication1.Helpers
{
    public class MessageParams:PaginationParams
    {
        public string? userName { get; set; }
        public string Container { get; set; } = "Unread";

    }
}
