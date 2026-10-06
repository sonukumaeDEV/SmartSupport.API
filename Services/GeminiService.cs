
using System.Text.Json;
using Google.GenAI;
using Google.GenAI.Types;
using SmartSupport.API.Interfaces;
using SmartSupport.API.Models;

namespace SmartSupport.API.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly Client _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GeminiService> _logger;

        public GeminiService(
            IConfiguration configuration,
            ILogger<GeminiService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var apiKey = _configuration["Gemini:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogError(
                    "Gemini API key is missing from configuration."
                );

                throw new InvalidOperationException(
                    "Gemini API key is not configured."
                );
            }

            _logger.LogInformation(
                "Gemini API key detected successfully."
            );

            _client = new Client(apiKey: apiKey);
        }

        public async Task<GeminiAnalysisResult> AnalyzeComplaintAsync(
            string title,
            string description)
        {
            var model =
                _configuration["Gemini:Model"]
                ?? "gemini-3.8-flash";

            _logger.LogInformation(
                "Starting Gemini complaint analysis. Model: {Model}",
                model
            );

            var prompt =
                """
                You are an AI customer support complaint analyzer.

                Analyze the customer complaint carefully.

                Return ONLY valid JSON.
                Do not use markdown.
                Do not use ```json.
                Do not add explanations before or after the JSON.

                The JSON must contain exactly these properties:

                {
                  "Category": "",
                  "Priority": "",
                  "Sentiment": "",
                  "Summary": "",
                  "SuggestedResponse": ""
                }

                Category MUST be one of:
                Technical Issue
                Payment
                Account
                Delivery
                Product
                Service
                Other

                Priority MUST be one of:
                Low
                Medium
                High
                Critical

                Sentiment MUST be one of:
                Positive
                Neutral
                Frustrated
                Angry
                Negative

                Summary:
                Give a short and clear summary of the complaint.

                SuggestedResponse:
                Give a professional, polite and helpful customer-support response.

                Customer Complaint:

                Title:
                """ + title + """

                Description:
                """ + description;

            try
            {
                _logger.LogInformation(
                    "Sending complaint to Gemini. Title: {Title}",
                    title
                );

                var response =
                    await _client.Models.GenerateContentAsync(
                        model: model,
                        contents: prompt,
                        config: new GenerateContentConfig
                        {
                            ResponseMimeType = "application/json"
                        }
                    );

                _logger.LogInformation(
                    "Gemini API request completed successfully."
                );

                var json = response.Text;

                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogError(
                        "Gemini returned an empty response."
                    );

                    throw new InvalidOperationException(
                        "Gemini returned an empty response."
                    );
                }

                json = CleanJsonResponse(json);

                _logger.LogInformation(
                    "Gemini response received and cleaned successfully."
                );

                var result =
                    JsonSerializer.Deserialize<GeminiAnalysisResult>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }
                    );

                if (result == null)
                {
                    _logger.LogError(
                        "Gemini response could not be deserialized."
                    );

                    throw new InvalidOperationException(
                        "Unable to parse Gemini response."
                    );
                }

                NormalizeResult(result);

                _logger.LogInformation(
                    "Gemini complaint analysis completed successfully. Category: {Category}, Priority: {Priority}, Sentiment: {Sentiment}",
                    result.Category,
                    result.Priority,
                    result.Sentiment
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Gemini complaint analysis failed. Model: {Model}",
                    model
                );

                throw new InvalidOperationException(
                    $"Gemini analysis failed: {ex.Message}",
                    ex
                );
            }
        }

        private static string CleanJsonResponse(string response)
        {
            response = response.Trim();

            if (response.StartsWith("```"))
            {
                var firstNewLine =
                    response.IndexOf('\n');

                if (firstNewLine >= 0)
                {
                    response =
                        response.Substring(
                            firstNewLine + 1
                        );
                }

                var closingFence =
                    response.LastIndexOf("```");

                if (closingFence >= 0)
                {
                    response =
                        response.Substring(
                            0,
                            closingFence
                        );
                }
            }

            response = response.Trim();

            var firstBrace =
                response.IndexOf('{');

            var lastBrace =
                response.LastIndexOf('}');

            if (firstBrace >= 0 &&
                lastBrace > firstBrace)
            {
                response =
                    response.Substring(
                        firstBrace,
                        lastBrace - firstBrace + 1
                    );
            }

            return response.Trim();
        }

        private static void NormalizeResult(
            GeminiAnalysisResult result)
        {
            var allowedCategories =
                new[]
                {
                    "Technical Issue",
                    "Payment",
                    "Account",
                    "Delivery",
                    "Product",
                    "Service",
                    "Other"
                };

            var allowedPriorities =
                new[]
                {
                    "Low",
                    "Medium",
                    "High",
                    "Critical"
                };

            var allowedSentiments =
                new[]
                {
                    "Positive",
                    "Neutral",
                    "Frustrated",
                    "Angry",
                    "Negative"
                };

            if (!allowedCategories.Contains(
                    result.Category,
                    StringComparer.OrdinalIgnoreCase))
            {
                result.Category = "Other";
            }

            if (!allowedPriorities.Contains(
                    result.Priority,
                    StringComparer.OrdinalIgnoreCase))
            {
                result.Priority = "Medium";
            }

            if (!allowedSentiments.Contains(
                    result.Sentiment,
                    StringComparer.OrdinalIgnoreCase))
            {
                result.Sentiment = "Neutral";
            }

            if (string.IsNullOrWhiteSpace(
                    result.Summary))
            {
                result.Summary =
                    "Customer complaint received and requires review.";
            }

            if (string.IsNullOrWhiteSpace(
                    result.SuggestedResponse))
            {
                result.SuggestedResponse =
                    "Thank you for contacting support. " +
                    "Our team will review your complaint " +
                    "and assist you shortly.";
            }
        }
    }
}











//using System.Text.Json;
//using Google.GenAI;
//using Google.GenAI.Types;
//using SmartSupport.API.Interfaces;
//using SmartSupport.API.Models;

//namespace SmartSupport.API.Services
//{
//    public class GeminiService : IGeminiService
//    {
//        private readonly Client _client;
//        private readonly IConfiguration _configuration;

//        public GeminiService(IConfiguration configuration)
//        {
//            _configuration = configuration;

//            var apiKey = _configuration["Gemini:ApiKey"];

//            if (string.IsNullOrWhiteSpace(apiKey))
//            {
//                throw new InvalidOperationException(
//                    "Gemini API key is not configured."
//                );
//            }

//            _client = new Client(apiKey: apiKey);
//        }

//        public async Task<GeminiAnalysisResult> AnalyzeComplaintAsync(
//            string title,
//            string description)
//        {
//            var model =
//                _configuration["Gemini:Model"]
//                ?? "gemini-3.8-flash";

//            var prompt =
//                """
//                You are an AI customer support complaint analyzer.

//                Analyze the customer complaint carefully.

//                Return ONLY valid JSON.
//                Do not use markdown.
//                Do not use ```json.
//                Do not add explanations before or after the JSON.

//                The JSON must contain exactly these properties:

//                {
//                  "Category": "",
//                  "Priority": "",
//                  "Sentiment": "",
//                  "Summary": "",
//                  "SuggestedResponse": ""
//                }

//                Category MUST be one of:
//                Technical Issue
//                Payment
//                Account
//                Delivery
//                Product
//                Service
//                Other

//                Priority MUST be one of:
//                Low
//                Medium
//                High
//                Critical

//                Sentiment MUST be one of:
//                Positive
//                Neutral
//                Frustrated
//                Angry
//                Negative

//                Summary:
//                Give a short and clear summary of the complaint.

//                SuggestedResponse:
//                Give a professional, polite and helpful customer-support response.

//                Customer Complaint:

//                Title:
//                """ + title + """

//                Description:
//                """ + description;

//            try
//            {
//                var response =
//                    await _client.Models.GenerateContentAsync(
//                        model: model,
//                        contents: prompt,
//                        config: new GenerateContentConfig
//                        {
//                            ResponseMimeType = "application/json"
//                        }
//                    );

//                var json = response.Text;

//                if (string.IsNullOrWhiteSpace(json))
//                {
//                    throw new InvalidOperationException(
//                        "Gemini returned an empty response."
//                    );
//                }

//                json = CleanJsonResponse(json);

//                var result =
//                    JsonSerializer.Deserialize<GeminiAnalysisResult>(
//                        json,
//                        new JsonSerializerOptions
//                        {
//                            PropertyNameCaseInsensitive = true
//                        }
//                    );

//                if (result == null)
//                {
//                    throw new InvalidOperationException(
//                        "Unable to parse Gemini response."
//                    );
//                }

//                NormalizeResult(result);

//                return result;
//            }
//            catch (Exception ex)
//            {
//                throw new InvalidOperationException(
//                    $"Gemini analysis failed: {ex.Message}",
//                    ex
//                );
//            }
//        }

//        private static string CleanJsonResponse(string response)
//        {
//            response = response.Trim();

//            if (response.StartsWith("```"))
//            {
//                var firstNewLine =
//                    response.IndexOf('\n');

//                if (firstNewLine >= 0)
//                {
//                    response =
//                        response.Substring(
//                            firstNewLine + 1
//                        );
//                }

//                var closingFence =
//                    response.LastIndexOf("```");

//                if (closingFence >= 0)
//                {
//                    response =
//                        response.Substring(
//                            0,
//                            closingFence
//                        );
//                }
//            }

//            response = response.Trim();

//            var firstBrace =
//                response.IndexOf('{');

//            var lastBrace =
//                response.LastIndexOf('}');

//            if (firstBrace >= 0 &&
//                lastBrace > firstBrace)
//            {
//                response =
//                    response.Substring(
//                        firstBrace,
//                        lastBrace - firstBrace + 1
//                    );
//            }

//            return response.Trim();
//        }

//        private static void NormalizeResult(
//            GeminiAnalysisResult result)
//        {
//            var allowedCategories =
//                new[]
//                {
//                    "Technical Issue",
//                    "Payment",
//                    "Account",
//                    "Delivery",
//                    "Product",
//                    "Service",
//                    "Other"
//                };

//            var allowedPriorities =
//                new[]
//                {
//                    "Low",
//                    "Medium",
//                    "High",
//                    "Critical"
//                };

//            var allowedSentiments =
//                new[]
//                {
//                    "Positive",
//                    "Neutral",
//                    "Frustrated",
//                    "Angry",
//                    "Negative"
//                };

//            if (!allowedCategories.Contains(
//                    result.Category,
//                    StringComparer.OrdinalIgnoreCase))
//            {
//                result.Category = "Other";
//            }

//            if (!allowedPriorities.Contains(
//                    result.Priority,
//                    StringComparer.OrdinalIgnoreCase))
//            {
//                result.Priority = "Medium";
//            }

//            if (!allowedSentiments.Contains(
//                    result.Sentiment,
//                    StringComparer.OrdinalIgnoreCase))
//            {
//                result.Sentiment = "Neutral";
//            }

//            if (string.IsNullOrWhiteSpace(
//                    result.Summary))
//            {
//                result.Summary =
//                    "Customer complaint received and requires review.";
//            }

//            if (string.IsNullOrWhiteSpace(
//                    result.SuggestedResponse))
//            {
//                result.SuggestedResponse =
//                    "Thank you for contacting support. " +
//                    "Our team will review your complaint " +
//                    "and assist you shortly.";
//            }
//        }
//    }
//}




