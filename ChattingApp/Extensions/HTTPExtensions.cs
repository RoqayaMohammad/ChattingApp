using ChattingApp.Helpers;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ChattingApp.Extensions
{
    public static class HTTPExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header)
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            response.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));
            response.Headers.Add("Access-Control-Expose-Header", "Pagination");
        }
    }
}
