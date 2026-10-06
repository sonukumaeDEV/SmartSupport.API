using SmartSupport.API.Models;

namespace SmartSupport.API.Interfaces
{
    public interface IGeminiService
    {
        Task<GeminiAnalysisResult> AnalyzeComplaintAsync(
            string title,
            string description);
    }
}
