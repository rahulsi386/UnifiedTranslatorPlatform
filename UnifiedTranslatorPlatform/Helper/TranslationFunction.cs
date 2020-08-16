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
            var functionUrl = $"{functionEndpoint}?code={functionApiKey}";
            var translationInput = new TranslationInput
            {
                TextInput = textInput,
                TargetLang = targetLang
            };
            var json = JsonConvert.SerializeObject(translationInput);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");
            using var httpClient = new HttpClient();
            //var result = await httpClient.GetStringAsync(functionUrl);

            var response = await httpClient.PostAsync(functionUrl, requestBody);
            var result = response.Content.ReadAsStringAsync().Result;
            return result;

        }
    }

    //Create a class to include all the properties that you want to pass as request body
    public class TranslationInput
    {
        public string TextInput { get; set; }
        public string TargetLang { get; set; }
    }
}
