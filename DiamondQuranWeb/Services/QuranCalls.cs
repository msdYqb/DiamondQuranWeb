using DiamondQuranWeb.Helpers;
using System.Runtime.InteropServices;

namespace DiamondQuranWeb.Services
{
    public class QuranCalls
    {
        public string CurrentDomain { get; set; }
        private readonly HttpClient client = new HttpClient();
        async Task<string> Send(Uri uri)
        {
            if (DockerHelper.InDocker)
            {
                var dockerPort = await DockerHelper.GetExposedPorts();
                uri.SetPort(dockerPort[0]);
            }

            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();
                return responseStr;
            }
            else return string.Empty;
        }
        public async Task<string> GetQuran(int currentSurah, int currentAyah, short currentPage, bool navByPage)
        {
            var builder = new UriBuilder(CurrentDomain + "/GetQuran")
            {
                Query = $"currentSurah={currentSurah}&currentAyah={currentAyah}&page={currentPage}&navByPage={navByPage}"
            };
            return await Send(builder.Uri);
        }
    }
}
