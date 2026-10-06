namespace SmartSupport.API.Models
{
    public class GeminiAnalysisResult
    {
        public string Category { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string Sentiment { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public string SuggestedResponse { get; set; } = string.Empty;
    }
}
