using TechAssignmentWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TechAssignmentWebApp.ApiRepository
{
    public class ApiServiceRepository
    {
        private HttpClient client { get; set; }

        public ApiServiceRepository()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebApiServiceUrl"].ToString());
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Accept", "application/json");

        }
                
        public Task<HttpResponseMessage> UploadFile(string url, FileUploadRequestModel reqModel)
        {
            try
            {
                               
                string requestJson = JsonConvert.SerializeObject(reqModel);
                HttpContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                var apiUrl = client.BaseAddress + url;
                var response = client.PostAsync(apiUrl, content).Result;

                return Task.FromResult(response);

            }
            catch (Exception ex)
            {
                var responseModel = new ResponseModel();
                responseModel.RespCode = "099";
                responseModel.RespDescription = "System Error [" + ex.Message + "]";

                var response = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(responseModel))
                };

                return Task.FromResult(response);

            }
        }

        public Task<HttpResponseMessage> GetAllUploadFile(string url)
        {
            try
            {


                var apiUrl = client.BaseAddress + url;
                var response = client.GetAsync(apiUrl).Result;

                return Task.FromResult(response);

            }
            catch (Exception ex)
            {
                var responseModel = new ResponseModel();
                responseModel.RespCode = "099";
                responseModel.RespDescription = "System Error [" + ex.Message + "]";

                var response = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(responseModel))
                };

                return Task.FromResult(response);

            }
        }
    }
}