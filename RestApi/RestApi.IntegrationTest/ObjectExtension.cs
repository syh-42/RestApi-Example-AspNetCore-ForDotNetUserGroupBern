using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace RestApi.IntegrationTest
{
    public static class ObjectExtension
    {
        public static StringContent ToJsonStringContent(this object content)
        {
            return new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
        }
    }
}