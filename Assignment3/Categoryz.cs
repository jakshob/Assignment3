using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EchoServer
{
    public class Categoryz
    {
        [JsonProperty("cid")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        
    }
}
