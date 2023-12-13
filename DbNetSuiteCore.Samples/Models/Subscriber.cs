namespace DbNetSuiteCoreApp.Models
{
    public class Subscriber
    {
        public string EmailAddress { get; set; }
        public bool Error { get; set; } = false;
        public string Message { get; set; }

    }
}
