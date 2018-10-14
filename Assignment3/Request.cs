using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EchoServer
{
    public class Request
    {
        [JsonProperty("method")]
        public string Method { get; set; }
        public string Path { get; set; }
        public string Date { get; set; }
        public string Body { get; set; }
    }
}
