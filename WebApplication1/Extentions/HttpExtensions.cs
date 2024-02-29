using System.Text.Json;
using WebApplication1.Helpers;

namespace WebApplication1.Extentions
{
    public static class HttpExtensions
    {
        public static void AppPaginationHeader(this HttpResponse response, PaginationHeader header)
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            response.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
