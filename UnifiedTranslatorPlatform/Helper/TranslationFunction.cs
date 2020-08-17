using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UnifiedTranslatorPlatform.Helper
{
    public class TranslationFunction
    {
        private const string _functionUrl = "FUNCTION_ENDPOINT_URL";
        private static readonly string functionEndpoint = Environment.GetEnvironmentVariable(_functionUrl);

        private const string _functionKey = "FUNCTION_API_KEY";
        private static readonly string functionApiKey = Environment.GetEnvironmentVariable(_functionKey);

        public static async Task<string> InvokeTranslationFunction(string textInput, string targetLang)
        {             
            var translationInput = new TranslationInput
            {
                TextInput = textInput,
                TargetLang = targetLang
            };
            var requestBody = JsonConvert.SerializeObject(translationInput);
            //var requestBody = new StringContent(json, Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    // Build the request.
                    // Set the method to Post.
                    request.Method = HttpMethod.Post;
                    // Construct the URI and add headers.
                    request.RequestUri = new Uri(functionEndpoint);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("X-Functions-Key", functionApiKey);

                    // Send the request and get response.
                    HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);
                    // Read response as a string.
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }            
        }
    }

    //Create a class to include all the properties that you want to pass as request body
    public class TranslationInput
    {
        public string TextInput { get; set; }
        public string TargetLang { get; set; }
    }

    public class TranslationOutput
    {
        public string toLang { get; set; }
        public string translatedText { get; set; }
        public string confidenceScore { get; set; }
    }
}
