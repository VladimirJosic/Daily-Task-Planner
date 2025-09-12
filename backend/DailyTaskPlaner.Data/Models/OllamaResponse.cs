using System.Text.Json.Serialization;

namespace DailyTaskPlaner.Data.Models;

public class OllamaResponse
{
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("response")]
    public string? Response { get; set; }

    [JsonPropertyName("done")]
    public bool Done { get; set; }

    [JsonPropertyName("done_reason")]
    public string DoneReason { get; set; }
}
