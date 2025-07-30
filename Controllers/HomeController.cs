using Microsoft.AspNetCore.Mvc;
using AI_TESTING.Models;
using System.Net.Http.Headers;

namespace AI_TESTING.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private const string OpenRouterKey = ""; // Replace this with your real key
    private const string OpenRouterEndpoint = "https://openrouter.ai/api/v1/chat/completions";

    public HomeController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Analyze(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            ModelState.AddModelError("", "Please upload an image.");
            return View("Index");
        }

        // Convert to Base64
        using var ms = new MemoryStream();
        await imageFile.CopyToAsync(ms);
        var base64Image = Convert.ToBase64String(ms.ToArray());

        // Build JSON payload
        var requestBody = new
        {
            model = "moonshotai/kimi-vl-a3b-thinking:free",
            messages = new[]
            {
                new { role = "user", content = new object[]
                    {
                        new
                        {
                            type = "text",
                            text = """
                        You are an OCR system. Your job is to extract **only** the readable data from an official ID card image and return it **strictly** in the following JSON format.

                        ABSOLUTELY NO commentary, NO explanations, NO surrounding text. Return ONLY valid JSON — nothing more.

                        Example output format:
                        {
                          "fullName": "John Doe",
                          "idNumber": "123456789",
                          "dateOfBirth": "1990-01-01",
                          "gender": "Male",
                          "address": "123 Main Street, Springfield",
                          "expiryDate": "2030-01-01"
                        }

                        If a field is missing, omit it. Do not guess. Do not add any extra fields. JUST JSON.
                        """
                        },

                        new { type = "image_url", image_url = new { url = $"data:{imageFile.ContentType};base64,{base64Image}" } }
                    }
                }
            }
        };

        var client = _clientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, OpenRouterEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", OpenRouterKey);
        request.Headers.Add("HTTP-Referer", "https://your-website-link");
        request.Headers.Add("X-Title", "AI_TESTING");

        request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestBody));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await client.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to analyze the image.");
            return View("Index");
        }

        var result = System.Text.Json.JsonDocument.Parse(json);
        var content = result.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();

        string cleaned = content!;

        int thinkStart = cleaned.IndexOf("◁think▷");
        int thinkEnd = cleaned.IndexOf("◁/think▷");

        if (thinkStart != -1 && thinkEnd != -1 && thinkEnd > thinkStart)
        {
            cleaned = cleaned.Remove(thinkStart, (thinkEnd + "◁/think▷".Length) - thinkStart).Trim();
        }

        cleaned = cleaned.Trim('`', '\n', '\r', ' ', '\t');

        var model = new ImageAnalysisResult
        {
            FileName = imageFile.FileName,
            Analysis = cleaned
        };


        return View("Result", model);
    }
}
