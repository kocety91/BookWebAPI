using System.Text.Json;

namespace BookWebAPI.Extensions
{
    public class ErrorDetails
    {
        public int Error { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
