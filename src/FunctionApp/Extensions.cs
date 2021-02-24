using System.Text;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;

namespace CleanArchitecture.FunctionApp
{
    public static class Extensions
    {
        public static T As<T>(this HttpRequestData httpRequestData)
        {
            var body = httpRequestData.Body.Value;
            var text = Encoding.UTF8.GetString(body.Span);
            
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}