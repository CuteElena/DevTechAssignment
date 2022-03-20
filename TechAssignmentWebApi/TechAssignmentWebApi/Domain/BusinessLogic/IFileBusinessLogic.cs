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
        Task<ResponseModel> UploadFile(FileUploadRequestModel reqModel);
        Task<ReportFilterResponseModel> GetTransactionsByCurrencyFilter(string currency);
        Task<ReportFilterResponseModel> GetTransactionsByStatusFilter(string status);
        Task<ReportFilterResponseModel> GetTransactionsByDateFilter(DateTime startDate, DateTime endDate);
    }
}
