using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechAssignmentWebApi.Domain.Models;
using TechAssignmentWebApi.Models;

namespace TechAssignmentWebApi.Domain.DataAccess
{
    public interface IFileDataAccess
    {
        Task<FileSaveResponseModel> SaveBulkFile(FileModel file);
        Task<ResponseModel> SaveUploadFileDeatail(List<UploadFileModel> fileRecords, int fileId);
        Task<DataTable> GetTransactionsByCurrencyFilter(string currency);
        Task<DataTable> GetTransactionsByStatusFilter(string status);
        Task<DataTable> GetTransactionsByDateFilter(DateTime startDate, DateTime endDate);
    }
}
