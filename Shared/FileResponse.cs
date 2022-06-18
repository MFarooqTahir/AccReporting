using System.Text.Json.Serialization;

namespace AccReporting.Shared
{
    public class FileResponse
    {
        [JsonPropertyName("File")]
        public byte[] File { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }
    }
}