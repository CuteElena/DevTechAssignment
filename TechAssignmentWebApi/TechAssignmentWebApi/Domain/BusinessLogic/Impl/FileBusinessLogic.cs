using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TechAssignmentWebApi.Domain.DataAccess;
using TechAssignmentWebApi.Domain.Models;
using TechAssignmentWebApi.Helpers;
using TechAssignmentWebApi.Models;

namespace TechAssignmentWebApi.Domain.BusinessLogic.Impl
{
    public class FileBusinessLogic : IFileBusinessLogic
    {
        private readonly IFileDataAccess _dataAccessBase;

        public FileBusinessLogic(IFileDataAccess dataAccessBase)
        {
            _dataAccessBase = dataAccessBase;

        }

        public async Task<ResponseModel> UploadFile(FileUploadRequestModel reqModel)
        {
            var resModel = new ResponseModel();
            try
            {

                var dt = new DataTable();
                if (reqModel.FileType.ToLower() == ".csv")
                {
                    dt = ApiUtility.ConvertCSVToDataTable(reqModel.FileContent);
                }
                else if (reqModel.FileType.ToLower() == ".xml")
                {
                    dt = ApiUtility.ConvertXMLToDataTable(reqModel.FileContent);
                }

                if (dt.Rows.Count == 0)
                {
                    resModel.RespCode = "014";
                    resModel.RespDescription = "Invalid File";
                    return resModel;
                }
                List<UploadFileModel> records = new List<UploadFileModel>();
                records = ApiUtility.ConvertDataTable<UploadFileModel>(dt);

                var checkResponse = ApiUtility.CheckDataValue(records, reqModel.FileType);
                if (checkResponse.RespCode != "000") return checkResponse;

                var fileType = reqModel.FileType.Replace(".", string.Empty);
                var fileModel = new FileModel(reqModel.FileName, fileType, reqModel.FileSize, records.Count, DateTime.Now);
                var saveReturn = await _dataAccessBase.SaveBulkFile(fileModel);
                if (saveReturn.RespCode != "000") return saveReturn;

                fileModel.Id = saveReturn.FileId;

                var detailSaveReturn = await _dataAccessBase.SaveUploadFileDeatail(records, fileModel.Id);
                if (detailSaveReturn.RespCode != "000") return detailSaveReturn;


                resModel.RespCode = "000";
                resModel.RespDescription = "Success";
                return resModel;
            }
            catch (Exception ex)
            {
                resModel.RespCode = "099";
                resModel.RespDescription = "Fail to Upload File with error[" + ex.Message + "]";
                return resModel;
            }
        }

        public async Task<ReportFilterResponseModel> GetTransactionsByCurrencyFilter(string currency)
        {
            var response = new ReportFilterResponseModel();
            var dataReturn = await _dataAccessBase.GetTransactionsByCurrencyFilter(currency);

            if (!ApiUtility.CheckNullorEmptyDataTable(dataReturn))
            {
                response.RespCode = "011";
                response.RespDescription = "No Record Found";
                return response;
            }
            response.lstData = dataReturn;
            response.RespCode = "000";
            response.RespDescription = "Success";
            return response;
        }

        public async Task<ReportFilterResponseModel> GetTransactionsByStatusFilter(string status)
        {
            var response = new ReportFilterResponseModel();
            var dataReturn = await _dataAccessBase.GetTransactionsByStatusFilter(status);

            if (!ApiUtility.CheckNullorEmptyDataTable(dataReturn))
            {
                response.RespCode = "011";
                response.RespDescription = "No Record Found";
                return response;
            }
            response.lstData = dataReturn;
            response.RespCode = "000";
            response.RespDescription = "Success";
            return response;
        }

        public async Task<ReportFilterResponseModel> GetTransactionsByDateFilter(DateTime startDate, DateTime endDate)
        {
            var response = new ReportFilterResponseModel();
            var dataReturn = await _dataAccessBase.GetTransactionsByDateFilter(startDate, endDate);

            if (!ApiUtility.CheckNullorEmptyDataTable(dataReturn))
            {
                response.RespCode = "011";
                response.RespDescription = "No Record Found";
                return response;
            }
            response.lstData = dataReturn;
            response.RespCode = "000";
            response.RespDescription = "Success";
            return response;
        }

    }
}