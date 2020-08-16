using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UnifiedTranslatorPlatform.Helper
{
    public class TranslationFunction
    {
        public static async Task<string> InvokeTranslationFunction(string textInput, string targetLang)
        {
            var functionUrl = "https://azureoefunctionapp.azurewebsites.net/api/Translator?code=b71HBssqaPnBsBNJtRdiN8KG1st7pfVoh9ZhBFQE3qbjKTDdjSKJOw==";
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
