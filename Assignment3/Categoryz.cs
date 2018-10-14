using Newtonsoft.Json;

namespace Assignment3
{
    public class Categoryz
    {
        [JsonProperty("cid")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        
    }
}
