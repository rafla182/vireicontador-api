using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using VireiContador.Infra.Helpers;

namespace VireiContador.Infra.Servico
{
    public class ServicoApi
    {
        public T GetData<T>(string url)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new JsonLowerCaseUnderscoreContractResolver()
            };

            using (var httpClient = new HttpClient())
            {

                var response = httpClient.GetAsync(url).Result;
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result, settings);
            }
        }

        public T GetDataVINDI<T>(string url)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new JsonLowerCaseUnderscoreContractResolver()
            };

            using (var httpClient = new HttpClient())
            {

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
            System.Text.ASCIIEncoding.ASCII.GetBytes(
               $"jJWHmrg564L0bUvJup5ul_eulUBI7sl9VwjqPFwCCW4")));


                var response = httpClient.GetAsync(url).Result;
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result, settings);
            }
        }

        public T GetDataRaw<T>(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(url).Result;
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            }
        }


        public void PostData(string url, string json, string apiKey)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new JsonLowerCaseUnderscoreContractResolver()
            };

            using (var httpClient = new HttpClient())
            {
                if(!(string.IsNullOrEmpty(apiKey)))
                    httpClient.DefaultRequestHeaders.Add("app-vireicontador	", "jJWHmrg564L0bUvJup5ul_eulUBI7sl9VwjqPFwCCW4");


                httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            }
        }

        public void PostDataToken(string url, string json, string token)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);

                httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            }
        }

        public T PostData<T>(string url, string json)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new JsonLowerCaseUnderscoreContractResolver()
            };

            using (var httpClient = new HttpClient())
            {
                var response = httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                return JsonConvert.DeserializeObject<T>(response.Result.Content.ReadAsStringAsync().Result, settings);
            }
        }

        public T PostDataRaw<T>(string url, string json)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                return JsonConvert.DeserializeObject<T>(response.Result.Content.ReadAsStringAsync().Result);
            }
        }

        public T PostDataAuth<T>(string url, string json)
        {

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new JsonLowerCaseUnderscoreContractResolver()
            };

            using (var httpClient = new HttpClient())
            {

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(
               $"jJWHmrg564L0bUvJup5ul_eulUBI7sl9VwjqPFwCCW4")));

                var response = httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result, settings);

            }
        }
    }
}
