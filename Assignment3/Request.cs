using Newtonsoft.Json;

namespace Assignment3
{
    public class Request
    {
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
