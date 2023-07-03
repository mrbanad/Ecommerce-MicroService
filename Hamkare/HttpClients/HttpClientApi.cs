using System.Net.Http.Headers;
using System.Text;
using CommonClassLibrary.Dto.Authentication.Login;
using Newtonsoft.Json;

namespace Hamkare.HttpClients;

public static class HttpClientApi
{
    static HttpClientApi()
    {
        Client = new HttpClient();
        Client.BaseAddress = new Uri("Https://localhost:44443");
        Client.Timeout = TimeSpan.FromMinutes(3);
    }

    private static HttpClient Client { get; }

    public static async Task<LoginResponseDto?> GetToken(LoginDto loginDto)
    {
        try
        {
            var myContent = JsonConvert.SerializeObject(loginDto);
            var buffer = Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await Client.PostAsync("/Identity/Authentication/Login", byteContent);
            return result.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<LoginResponseDto>(await result.Content.ReadAsStringAsync())
                : null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}