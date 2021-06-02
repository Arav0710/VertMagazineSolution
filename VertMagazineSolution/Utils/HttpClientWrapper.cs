using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VertMagazineSolution.Model;

namespace VertMagazineSolution.Utils
{
    public class HttpClientWrapper
    {
        private readonly HttpClient _httpClient;
        private string _token;

        public HttpClientWrapper()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Accept",
            "application/json");
            _token = string.Empty;
        }

        /// <summary>
        /// gets result from given URL
        /// </summary>
        /// <typeparam name="T">type of result object</typeparam>
        /// <param name="url">url to get the result</param>
        /// <returns></returns>
        public async Task<Results<T>> Get<T>(string url)
        {
            
            if (string.IsNullOrEmpty(_token))
            {
                await GetTokenAsync();
            }

            var reponse = await GetFromApiAsync<Results<T>>(url);

            //ensures api doesnt fail because of auth issue
            while (!reponse.success && reponse.message.Contains("invalid token", StringComparison.OrdinalIgnoreCase)) 
            {
                await GetTokenAsync();
                reponse = await GetFromApiAsync<Results<T>>(url);
            } 


            return reponse;
        }


        /// <summary>
        /// Post the request to given URL
        /// </summary>
        /// <typeparam name="T">type of return object</typeparam>
        /// <typeparam name="Y">type of body object</typeparam>
        /// <param name="url">url to request</param>
        /// <param name="body">body to be posted</param>
        /// <returns></returns>
        public async Task<PostResults<T>> Post<T,Y>(string url,Y body)
        {
            if (string.IsNullOrEmpty(_token))
            {
                await GetTokenAsync();
            }

            var reponse = await PostFromApiAsync<PostResults<T>,Y>(url,body);

            //ensures api doesnt fail because of auth issue
            while (!reponse.success && reponse.message.Contains("invalid token", StringComparison.OrdinalIgnoreCase))
            {
                await GetTokenAsync();
                reponse = await PostFromApiAsync<PostResults<T>,Y>(url,body);
            }


            return reponse;
        }

        #region private

        private async Task<T> GetFromApiAsync<T>(string url)
        {
            url = url.Replace("#token#", _token);
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync
                        <T>(responseStream);              

            }
            throw new Exception($"Error while getting results with message: {response.ReasonPhrase}"); ;
        }
        private async Task<T> PostFromApiAsync<T,Y>(string url,Y body)
        {
            url = url.Replace("#token#", _token);
            var bodyContent = new StringContent(JsonSerializer.Serialize(body),Encoding.UTF8,"application/json");
            var response = await _httpClient.PostAsync(url, bodyContent);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync
                        <T>(responseStream);

            }
            throw new Exception($"Error while getting results with message: {response.ReasonPhrase}"); ;
        }

        private async Task GetTokenAsync()
        {
            var tokenResponse = await _httpClient.GetAsync(API.Token);

            if (tokenResponse.IsSuccessStatusCode)
            {
                using var responseStream = await tokenResponse.Content.ReadAsStreamAsync();
                var tokenResult = await JsonSerializer.DeserializeAsync
                        <Token>(responseStream);

                _token = tokenResult.success ? tokenResult.token : throw new Exception($"Error while getting token with message: {tokenResult.message}"); 
                return;

            }
            throw new Exception($"Error while getting token with message: {tokenResponse.ReasonPhrase}"); ;
        }
        #endregion
    }
}
