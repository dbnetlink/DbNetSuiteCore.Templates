using Newtonsoft.Json;

namespace DbNetSuiteCoreApp.Models
{
    public partial class SenderSubscriber
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("groups")]
        public string[] Groups { get; set; } 
    }
}
