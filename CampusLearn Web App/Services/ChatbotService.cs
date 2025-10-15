using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CampusLearn_Web_App.Services
{
    public class ChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly Dictionary<string, object> _appKnowledge;

        public ChatbotService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;

            // Load your dataset (app_knowledge.json) into memory
            var knowledgePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "appKnowledge.json");

            if (File.Exists(knowledgePath))
            {
                var jsonData = File.ReadAllText(knowledgePath);
                _appKnowledge = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData)
                    ?? new Dictionary<string, object>();
            }
            else
            {
                Console.WriteLine("⚠️ Warning: appKnowledge.json not found in /Data folder.");
                _appKnowledge = new Dictionary<string, object>();
            }
        }

        public async Task<string> GetChatResponseAsync(string userMessage)
        {
            var apiKey = _config["Chatbot:ApiKey"];
            var model = _config["Chatbot:Model"];
            var endpoint = _config["Chatbot:Endpoint"];

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            // Combine dataset + user question into context prompt
            var datasetSummary = JsonSerializer.Serialize(_appKnowledge, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var prompt = $@"
You are a helpful assistant for the CampusLearn platform.
Your role is to help users understand how to use the app — how navigation, roles, dashboards, and features work.
Use ONLY the information in the dataset below to answer questions. 
If the dataset doesn’t contain the answer, politely say you don’t have that information.

DATASET:
{datasetSummary}

USER QUESTION:
{userMessage}
";

            var requestBody = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "system", content = "You are a CampusLearn assistant trained on the app dataset." },
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Chatbot API error: {response.StatusCode} - {responseString}");
                return $"⚠️ Error: Chatbot API returned {response.StatusCode}";
            }

            using var doc = JsonDocument.Parse(responseString);

            // Handle OpenAI and Hugging Face response formats
            if (doc.RootElement.TryGetProperty("choices", out var choices))
            {
                // OpenAI response
                return choices[0].GetProperty("message").GetProperty("content").GetString()?.Trim()
                       ?? "⚠️ Empty response.";
            }
            else if (doc.RootElement.ValueKind == JsonValueKind.Array &&
                     doc.RootElement[0].TryGetProperty("generated_text", out var generated))
            {
                // Hugging Face response
                return generated.GetString()?.Trim() ?? "⚠️ Empty response.";
            }

            return "⚠️ Unexpected response format from chatbot API.";
        }
    }
}
