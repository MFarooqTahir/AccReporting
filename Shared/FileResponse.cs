using System.Text.Json.Serialization;

namespace AccReporting.Shared
{
    public class FileResponse
    {
        [JsonPropertyName(name: nameof(File))]
        public byte[] File { get; set; }

        [JsonPropertyName(name: nameof(Name))]
        public string Name { get; set; }
    }
}