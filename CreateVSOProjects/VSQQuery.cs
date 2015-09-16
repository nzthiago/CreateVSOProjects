using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace VSOCommon
{
    public class VSOQuery : IDisposable
    {
        private string _instance;
        private string _userName;
        private string _password;
        private HttpClient _client;

        public VSOQuery(string instance, string userName, string password) 
        {
            _instance = instance;
            _userName = userName;
            _password = password;

            AuthenticationHeaderValue _authHeader = new AuthenticationHeaderValue("Basic",
                                Convert.ToBase64String(Encoding.ASCII.GetBytes(
                                    string.Format("{0}:{1}", userName, password))));
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

            _client.DefaultRequestHeaders.Authorization = _authHeader;
        }

        public async Task<dynamic> GrabJSONDataWithPath(string path)
        {
            string url = "https://" + _instance + "/" + path;
            return await GrabJSONData(url);
        }
        public async Task<dynamic> GrabJSONDataWithPathAndPost(string path, HttpContent content, CookieContainer cookies)
        {
            string url = "https://" + _instance + "/" + path;
            return await GrabJSONDataWithPost(url, content, cookies);
        }
        public async Task<dynamic> GrabJSONData(string url)
        {
            using (HttpResponseMessage response = await _client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                dynamic obj = JToken.Parse(responseBody);
                return obj;
            }
        }
        public async Task<dynamic> GrabJSONDataWithPost(string url, HttpContent content, CookieContainer cookies)
        {
            using (var handler = new HttpClientHandler() { CookieContainer = cookies })
            {
                using (HttpResponseMessage response = await _client.PostAsync(url, content))
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    dynamic obj = JValue.Parse(responseBody);
                    return obj;
                }
            }
        }
        public async Task<string> GrabRawData(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", _userName, _password))));

                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
        public void Dispose()
        {
            ((IDisposable)_client).Dispose();
        }
    }
}
