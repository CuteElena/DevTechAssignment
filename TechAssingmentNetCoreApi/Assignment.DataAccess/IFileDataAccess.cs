using Assignment.Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.DataAccess
{
    public interface IFileDataAccess
    {
        Task<FileSaveResponseModel> SaveBulkFile(FileModel file);
        Task<ResponseModel> SaveUploadFileDeatail(List<UploadFileModel> fileRecords, int fileId);
        Task<DataTable> GetAllUploadFile();
        Task<DataTable> GeUploadFileById(int fileId);
        Task<DataTable> GetAllUploadFileDetailByFileId(int fileId);
        Task<DataTable> GetTransactionsByCurrencyFilter(string currency);
        Task<DataTable> GetTransactionsByStatusFilter(string status);
        Task<DataTable> GetTransactionsByDateFilter(DateTime startDate, DateTime endDate);
    }
}
