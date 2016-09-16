using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestApi.IntegrationTest
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            return await content.ReadAsStringAsync().ContinueWith(t => JsonConvert.DeserializeObject<T>(t.Result));
        }
    }
}