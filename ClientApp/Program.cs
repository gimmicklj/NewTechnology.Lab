using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClientApp
{
    class Program
    {
        private static string gateway = "https://localhost:44315";

        static void Main(string[] args)
        {
            CallWebAPI_WithoutToken();
            CallWebAPI_WithToken();

            Console.Read();
        }

        /// <summary>
        /// 无Token时访问WebAPI
        /// </summary>
        private static void CallWebAPI_WithoutToken()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(gateway);

            // 访问WebAPI时无认证的token，网关将返回 401 状态.
            Console.Write($"向WebAPI发送消息（无token）,");
            var resWithoutToken = client.GetAsync("/webapi-1/todoitems").Result;

            Console.WriteLine($"返回结果 : {(int)resWithoutToken.StatusCode}, {resWithoutToken.ReasonPhrase}");
        }

        /// <summary>
        /// 无Token时访问WebAPI
        /// </summary>
        private static void CallWebAPI_WithToken()
        {
            var token = GetJwtToken();
            Console.WriteLine($"-----------------------------------");
            Console.WriteLine($"JWT Token = {token}");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(gateway);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            
            
            Console.WriteLine($"\n-------请求WebAPI-------");
            var resWithToken = client.GetAsync("/webapi-1/todoitems").Result;
            
            Console.WriteLine($"返回结果 : {(int)resWithToken.StatusCode}, {resWithToken.Content.ReadAsStringAsync().Result}");

        }

        private static string GetJwtToken()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(gateway);
            client.DefaultRequestHeaders.Clear();

            string username = "admin";
            string password = "123456";

            var res = client.GetAsync($"/auth?username={username}&password={password}").Result;
            var json = res.Content.ReadAsStringAsync().Result;

            var jwt = JsonSerializer.Deserialize<JWT>(json);

            return jwt.access_token;
        }
    }


}