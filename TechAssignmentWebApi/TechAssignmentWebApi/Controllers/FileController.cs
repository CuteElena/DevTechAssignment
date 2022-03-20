using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TechAssignmentWebApi.Domain.BusinessLogic;
using TechAssignmentWebApi.Helpers;
using TechAssignmentWebApi.Models;

namespace TechAssignmentWebApi.Controllers
{
    public class FileController : ApiController
    {
        private HttpResponseMessage response;
        private readonly IFileBusinessLogic _businessLogic;

        public FileController(IFileBusinessLogic businessLogic)
        {
            _businessLogic = businessLogic;
        }

        [HttpPost]
        [Route("API/UploadFileAsCsvByte")]
        public async Task<HttpResponseMessage> UploadFileAsCsvByte()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }
            string[] extensions = { ".csv", ".xml" };
            var file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

            if (!extensions.Any(x => x.Equals(Path.GetExtension(file.FileName.ToLower()), StringComparison.OrdinalIgnoreCase)))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);//"Invalid file type.", 
            }

            var dataReturn = new FileUploadRequestModel();
            if (file != null && file.ContentLength > 0)
            {
                // var fileName = Path.GetFileName(file.FileName);
                dataReturn.FileName = file.FileName;
                dataReturn.FileSize = file.ContentLength;
                dataReturn.FileType = Path.GetExtension(file.FileName.ToLower());

                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    dataReturn.FileContent = binaryReader.ReadBytes(file.ContentLength);
                }


            }
            var response = new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(dataReturn))
            };

            return await Task.FromResult(response);


        }


        [HttpPost]
        [Route("API/UploadFile")]
        public async Task<HttpResponseMessage> UploadBulkFileAsync([FromBody] FileUploadRequestModel requestModel)
        {
            var startTime = DateTime.Now;
            string errMsg = "";
            var respData = "";
            var apiUtility = new ApiUtility();


            try
            {

                #region Validation                              

                if (string.IsNullOrEmpty(requestModel.FileName))
                {
                    errMsg = "File Name is required.";
                    return await Task.FromResult(apiUtility.GetHttpAPIResponse("012", errMsg));
                }
                if (string.IsNullOrEmpty(requestModel.FileType))
                {
                    errMsg = "File Name is required.";
                    return await Task.FromResult(apiUtility.GetHttpAPIResponse("012", errMsg));
                }

                if (requestModel.FileSize > 1000000)
                {
                    errMsg = "Maximum File size is only 1 MB.";
                    return await Task.FromResult(apiUtility.GetHttpAPIResponse("012", errMsg));
                }
                if (requestModel.FileContent == null)
                {
                    errMsg = "File Content is required.";
                    return await Task.FromResult(apiUtility.GetHttpAPIResponse("012", errMsg));
                }

                if (!(requestModel.FileType.ToLower() == ".csv" || requestModel.FileType.ToLower() == ".xml"))
                {
                    errMsg = "Unknown format";
                    return await Task.FromResult(apiUtility.GetHttpAPIResponse("012", errMsg));
                }

                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(requestModel.FileName);
                if (fileNameWithoutExtension.Length > 250)
                {
                    errMsg = "You have entered an invalid filename length. The Filename length must not exceed 250 characters.";
                    return await Task.FromResult(apiUtility.GetHttpAPIResponse("012", errMsg));

                }


                #endregion

                #region CheckFileValidation

                var dataReturn = await _businessLogic.UploadFile(requestModel);
                if (dataReturn.RespCode != "000")
                {
                    errMsg = dataReturn.RespDescription;
                }

                response = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(dataReturn))
                };
                respData = JsonConvert.SerializeObject(dataReturn);
                return await Task.FromResult(response);

                #endregion


            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                var requestFilePath = ApiUtility.ExceptionLineAndFile(ex);
                var response = apiUtility.GetAPIExceptionResponse(ex);
                return await Task.FromResult(response);
            }

        }

        [HttpPost]
        [Route("API/GetTransactionsByCurrency")]
        public async Task<HttpResponseMessage> GetTransactionsByCurrencyFilterAsync([FromBody] ReportFilterModel requestModel)
        {
            var startTime = DateTime.Now;
            string errMsg = "";
            var respData = "";
            var apiUtility = new ApiUtility();


            try
            {

                #region Validation                              

                if (string.IsNullOrEmpty(requestModel.Currency))
                {
                    errMsg = "Currency is required.";
                    return await Task.FromResult(apiUtility.GetHttpAPIResponse("012", errMsg));
                }

                #endregion

                #region CheckFileValidation

                var dataReturn = await _businessLogic.GetTransactionsByCurrencyFilter(requestModel.Currency);
                if (dataReturn.RespCode != "000")
                {
                    errMsg = dataReturn.RespDescription;
                }

                response = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(dataReturn))
                };
                respData = JsonConvert.SerializeObject(dataReturn);
                return await Task.FromResult(response);

                #endregion


            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                var requestFilePath = ApiUtility.ExceptionLineAndFile(ex);
                var response = apiUtility.GetAPIExceptionResponse(ex);
                return await Task.FromResult(response);
            }

        }

        [HttpPost]
        [Route("API/GetTransactionsByStatus")]
        public async Task<HttpResponseMessage> GetTransactionsByStatusFilterAsync([FromBody] ReportFilterModel requestModel)
        {
            var startTime = DateTime.Now;
            string errMsg = "";
            var respData = "";
            var apiUtility = new ApiUtility();


            try
            {

                #region Validation                              

                if (string.IsNullOrEmpty(requestModel.Status))
                {
                    errMsg = "Status is required.";
                    return await Task.FromResult(apiUtility.GetHttpAPIResponse("012", errMsg));
                }

                #endregion

                #region CheckFileValidation

                var dataReturn = await _businessLogic.GetTransactionsByStatusFilter(requestModel.Status);
                if (dataReturn.RespCode != "000")
                {
                    errMsg = dataReturn.RespDescription;
                }

                response = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(dataReturn))
                };
                respData = JsonConvert.SerializeObject(dataReturn);
                return await Task.FromResult(response);

                #endregion


            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                var requestFilePath = ApiUtility.ExceptionLineAndFile(ex);
                var response = apiUtility.GetAPIExceptionResponse(ex);
                return await Task.FromResult(response);
            }

        }

        [HttpPost]
        [Route("API/GetTransactionsByDate")]
        public async Task<HttpResponseMessage> GetTransactionsByDateFilterAsync([FromBody] ReportFilterModel requestModel)
        {
            var startTime = DateTime.Now;
            string errMsg = "";
            var respData = "";
            var apiUtility = new ApiUtility();


            try
            {

                #region Validation                              

                if (requestModel.StartDate == null)
                {
                    errMsg = "Start Date is required.";
                    return await Task.FromResult(apiUtility.GetHttpAPIResponse("012", errMsg));
                }

                if (requestModel.EndDate == null)
                {
                    errMsg = "End Date is required.";
                    return await Task.FromResult(apiUtility.GetHttpAPIResponse("012", errMsg));
                }

                #endregion

                #region CheckFileValidation

                var dataReturn = await _businessLogic.GetTransactionsByDateFilter(requestModel.StartDate, requestModel.EndDate);
                if (dataReturn.RespCode != "000")
                {
                    errMsg = dataReturn.RespDescription;
                }

                response = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(dataReturn))
                };
                respData = JsonConvert.SerializeObject(dataReturn);
                return await Task.FromResult(response);

                #endregion


            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                var requestFilePath = ApiUtility.ExceptionLineAndFile(ex);
                var response = apiUtility.GetAPIExceptionResponse(ex);
                return await Task.FromResult(response);
            }

        }

    }
}