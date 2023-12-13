using Newtonsoft.Json;

namespace DbNetSuiteCoreApp.Models
{
    public class SenderResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
