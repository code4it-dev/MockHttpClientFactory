using System.Net.Http;
using System.Threading.Tasks;

namespace Libs
{

    public class MyExternalService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MyExternalService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task DeleteObject(string objectName)
        {
            string path = $"/objects?name={objectName}";
            var client = _httpClientFactory.CreateClient("ext_service");

            var httpResponse = await client.DeleteAsync(path);

            httpResponse.EnsureSuccessStatusCode();
        }
    }
}
