using System.Text.Json.Serialization;

namespace WebApi.Models.Responses;

public class AiStatisticsResponse
{
    [JsonPropertyName("ratings")]
    public ChartData Ratings { get; set; } = new();

    [JsonPropertyName("clients")]
    public ChartData Clients { get; set; } = new();

    [JsonPropertyName("sentiments")]
    public ChartData Sentiments { get; set; } = new();
}

public class ChartData
{
    [JsonPropertyName("labels")]
    public string[] Labels { get; set; } = Array.Empty<string>();

    [JsonPropertyName("data")]
    public int[] Data { get; set; } = Array.Empty<int>();
}

public class ReviewAiModel
{
    [JsonPropertyName("raw_text")]
    public string RawText { get; set; } = string.Empty;

    [JsonPropertyName("submitted_on")]
    public string SubmittedOn { get; set; } = string.Empty;

    [JsonPropertyName("rating")]
    public double Rating { get; set; }

    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = string.Empty;

    [JsonPropertyName("rating_type")]
    public string RatingType { get; set; } = string.Empty;

    [JsonPropertyName("sentiment_label")]
    public string SentimentLabel { get; set; } = string.Empty;
}
