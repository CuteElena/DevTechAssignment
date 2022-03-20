using Assignment.Common.Helpers;
using Assignment.Common.Models;
using Assignment.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace Assignment.Business.Impl
{
    public class FileBusinessLogic : IFileBusinessLogic
    {
        private readonly IFileDataAccess _dataAccessBase;
        private readonly IConfiguration _configuration;
        public FileBusinessLogic(IFileDataAccess dataAccessBase , IConfiguration configuration)
        {
            _dataAccessBase = dataAccessBase;
            _configuration = configuration;

        }

        public async Task<FileUploadResponseModel> UploadFile(FileUploadRequestModel reqModel)
        {
            string projectBulkFilepath = projectBulkFilepath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..\\")) + _configuration["BulkUploadFilesPath"]?.ToString();
            if (!Directory.Exists(projectBulkFilepath))
            {
                Directory.CreateDirectory(projectBulkFilepath);
            }

            var today = DateTime.UtcNow.ToString("ddMMyyyy");
            var path = projectBulkFilepath + @"\" + today;

            FileDrop(path + @"\" + "Upload", reqModel.FileName + reqModel.FileType, reqModel.FileContent);

            var resModel = new FileUploadResponseModel();
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
                    FileDrop(path + @"\" + "Error", reqModel.FileName + reqModel.FileType, null);

                    resModel.RespCode = "014";
                    resModel.RespDescription = "Invalid File";
                    return resModel;
                }
                List<UploadFileModel> records = new List<UploadFileModel>();
                records = ApiUtility.ConvertDataTable<UploadFileModel>(dt);

                var checkResponse = ApiUtility.CheckDataValue(records, reqModel.FileType);
                if (checkResponse.RespCode != "000")
                {
                    FileDrop(path + @"\" + "Error", reqModel.FileName + reqModel.FileType, null);

                    resModel.RespCode = checkResponse.RespCode;
                    resModel.RespDescription = checkResponse.RespDescription;
                    return resModel;
                }

                var fileType = reqModel.FileType.Replace(".", string.Empty);
                var fileModel = new FileModel(reqModel.FileName, fileType, reqModel.FileSize, records.Count, DateTime.Now);
                var saveReturn = await _dataAccessBase.SaveBulkFile(fileModel);
                if (saveReturn.RespCode != "000")
                {
                    resModel.RespCode = saveReturn.RespCode;
                    resModel.RespDescription = saveReturn.RespDescription;
                    return resModel;
                }

                fileModel.Id = saveReturn.FileId;

                var detailSaveReturn = await _dataAccessBase.SaveUploadFileDeatail(records, fileModel.Id);
                if (detailSaveReturn.RespCode != "000")
                {
                    resModel.RespCode = detailSaveReturn.RespCode;
                    resModel.RespDescription = detailSaveReturn.RespDescription;
                    return resModel;

                }

                FileDrop(path + @"\" + "Success", reqModel.FileName + reqModel.FileType, null);

                resModel.RespCode = "000";
                resModel.RespDescription = "File Upload Successfully";
                resModel.TotalRecords = records.Count;
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

        public async Task<ReportFilterResponseModel> GetAllUploadFile()
        {
            var response = new ReportFilterResponseModel();
            var dataReturn = await _dataAccessBase.GetAllUploadFile();

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

        public async Task<ReportFilterResponseModel> GetAllUploadFileDetailByFileId(int fileId)
        {
            var response = new ReportFilterResponseModel();
            var fReturn = await _dataAccessBase.GeUploadFileById(fileId);

            if (!ApiUtility.CheckNullorEmptyDataTable(fReturn))
            {
                response.RespCode = "013";
                response.RespDescription = "Invalid File Id";
                return response;

            }

            var dataReturn = await _dataAccessBase.GetAllUploadFileDetailByFileId(fileId);
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

        private void FileDrop(string path, string fileName, byte[] fileContent)
        {

            if (File.Exists(path + @"\" + fileName)) File.Delete(path + @"\" + fileName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }


            if (path.Contains("Success") || path.Contains("Error"))
            {
                var destFolder = string.Empty;
                if (path.Contains("Success")) destFolder = "Success";
                else if (path.Contains("Error")) destFolder = "Error";

                var destinationPath = path + @"\" + fileName;
                var sourcePath = path.Replace(destFolder, "Upload") + @"\" + fileName;

                File.Move(sourcePath, destinationPath);
            }
            else File.WriteAllBytes(path + @"\" + fileName, fileContent);
        }
    }
}