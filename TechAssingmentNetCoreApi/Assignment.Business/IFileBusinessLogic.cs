using Assignment.Common.Models;
using System;
using System.Threading.Tasks;

namespace Assignment.Business
{
    public interface IFileBusinessLogic
    {
        Task<FileUploadResponseModel> UploadFile(FileUploadRequestModel reqModel);
        Task<ReportFilterResponseModel> GetAllUploadFile();
        Task<ReportFilterResponseModel> GetAllUploadFileDetailByFileId(int fileId);
        Task<ReportFilterResponseModel> GetTransactionsByCurrencyFilter(string currency);
        Task<ReportFilterResponseModel> GetTransactionsByStatusFilter(string status);
        Task<ReportFilterResponseModel> GetTransactionsByDateFilter(DateTime startDate, DateTime endDate);
    }
}
