using Assignment.Business;
using Assignment.Common.Helpers;
using Assignment.Common.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TechAssingmentNetCoreApi.Controllers
{
    [ApiController]
    public class FileController : Controller
    {
        private readonly IFileBusinessLogic _businessLogic;

        public FileController(IFileBusinessLogic businessLogic)
        {
            _businessLogic = businessLogic;
        }

        [HttpPost]
        [Route("API/UploadFile")]
        public async Task<IActionResult> UploadBulkFileAsync([FromBody] FileUploadRequestModel requestModel)
        {           
            string errMsg = "";
            var response = new FileUploadResponseModel();
          
            try
            {

                #region Validation                              

                if (string.IsNullOrEmpty(requestModel.FileName))
                {
                    errMsg = "File Name is required.";
                    return Json(new ResponseModel("12", errMsg));
                   
                }
                if (string.IsNullOrEmpty(requestModel.FileType))
                {
                    errMsg = "File Name is required.";
                    return Json(new ResponseModel("12", errMsg));
                }

                if (requestModel.FileSize > 1000000)
                {
                    errMsg = "Maximum File size is only 1 MB.";
                    return Json(new ResponseModel("12", errMsg));
                }
                if (requestModel.FileContent == null)
                {
                    errMsg = "File Content is required.";
                    return Json(new ResponseModel("12", errMsg));
                }

                if (!(requestModel.FileType.ToLower() == ".csv" || requestModel.FileType.ToLower() == ".xml"))
                {
                    errMsg = "Unknown format";
                    return Json(new ResponseModel("12", errMsg));
                }

                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(requestModel.FileName);
                if (fileNameWithoutExtension.Length > 250)
                {
                    errMsg = "You have entered an invalid filename length. The Filename length must not exceed 250 characters.";
                    return Json(new ResponseModel("12", errMsg));

                }


                #endregion
                               
                response = await _businessLogic.UploadFile(requestModel);                
               
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;               
                return Json(new ResponseModel("12", errMsg));
            }
            return Json(response);
        }

        [HttpGet]
        [Route("API/GetAllUploadFiles")]
        public async Task<IActionResult> GetAllUploadFiles()
        {            
            string errMsg = "";
            var response = new ReportFilterResponseModel();           

            try
            {
               
                 response = await _businessLogic.GetAllUploadFile();  

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return Json(new ResponseModel("12", errMsg));
            }
            return Json(response);
        }

        [HttpPost]
        [Route("API/GeAllFileDetailsByFile")]
        public async Task<IActionResult> GeAllFileDetailsByFile([FromBody] FileDetailRequestModel requestModel)
        {           
            string errMsg = "";
            var response = new ReportFilterResponseModel();
            try
            {


                #region Validation                              

                if (requestModel.FileId <= 0)
                {
                    errMsg = "File Id is required.";
                    return Json(new ResponseModel("12", errMsg));
                }

                #endregion

               response = await _businessLogic.GetAllUploadFileDetailByFileId(requestModel.FileId);
                

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return Json(new ResponseModel("12", errMsg));
            }
            return Json(response);

        }
        [HttpPost]
        [Route("API/GetTransactionsByCurrency")]
        public async Task<IActionResult> GetTransactionsByCurrencyFilterAsync([FromBody] ReportFilterModel requestModel)
        {           
            string errMsg = "";
            var response = new ReportFilterResponseModel();

            try
            {

                #region Validation                              

                if (string.IsNullOrEmpty(requestModel.Currency))
                {
                    errMsg = "Currency is required.";
                    return Json(new ResponseModel("12", errMsg));
                }

                #endregion


                response = await _businessLogic.GetTransactionsByCurrencyFilter(requestModel.Currency);
                

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return Json(new ResponseModel("12", errMsg));
            }
            return Json(response);

        }

        [HttpPost]
        [Route("API/GetTransactionsByStatus")]
        public async Task<IActionResult> GetTransactionsByStatusFilterAsync([FromBody] ReportFilterModel requestModel)
        {           
            string errMsg = "";
            var response = new ReportFilterResponseModel();

            try
            {

                #region Validation                              

                if (string.IsNullOrEmpty(requestModel.Status))
                {
                    errMsg = "Status is required.";
                    return Json(new ResponseModel("12", errMsg));
                }

                #endregion

                response = await _businessLogic.GetTransactionsByStatusFilter(requestModel.Status);
                
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return Json(new ResponseModel("12", errMsg));
            }
            return Json(response);

        }

        [HttpPost]
        [Route("API/GetTransactionsByDate")]
        public async Task<IActionResult> GetTransactionsByDateFilterAsync([FromBody] ReportFilterModel requestModel)
        {           
            string errMsg = "";
            var response = new ReportFilterResponseModel();

            try
            {

                #region Validation                              
                               
                if (requestModel.StartDate == null)
                {
                    errMsg = "Start Date is required.";
                    return Json(new ResponseModel("12", errMsg));
                }

                if (requestModel.EndDate == null)
                {
                    errMsg = "End Date is required.";
                    return Json(new ResponseModel("12", errMsg));
                }

                #endregion

                response = await _businessLogic.GetTransactionsByDateFilter(requestModel.StartDate, requestModel.EndDate);
                
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return Json(new ResponseModel("12", errMsg));
            }
            return Json(response);

        }

    }
}
