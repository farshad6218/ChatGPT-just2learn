using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenAI_API;


namespace FKHxGPT.Controllers
{
    [Route("api/chatgpt")]
    [ApiController]
    public class ChatGptController : ControllerBase
    {
        //this is not a valid api key, replace with your own key
        string apiKey = "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";
        private readonly HttpClient httpClient;

        public ChatGptController()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Author", $"Farshad Khadempour");
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ChatRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Message))
                {
                    return BadRequest("Message cannot be empty.");
                }

                string response = string.Empty;
                OpenAIAPI api = new OpenAIAPI(apiKey); // shorthand

                // Define the conversation with ChatGPT
                var chat = api.Chat.CreateConversation();

                /// give instruction as System
                chat.AppendSystemMessage("You are a helpful assistant.");

                // give a few examples as user and assistant
                chat.AppendUserInput(request.Message);

                // Send a POST request to the ChatGPT API
                // and get the response

                await foreach (var res in chat.StreamResponseEnumerableFromChatbotAsync())
                {
                    response = (res) + "<br/>" + response;
                }

                if (response != string.Empty)
                {
                    string assistantReply = response;
                    return Ok(new { Reply = assistantReply });
                }
                else
                {
                    return BadRequest("Error sending message to ChatGPT API");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class ChatRequest
    {
        public string? Message { get; set; }
    }
}