using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechAssignmentWebApi.Models;

namespace TechAssignmentWebApi.Domain.BusinessLogic
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
