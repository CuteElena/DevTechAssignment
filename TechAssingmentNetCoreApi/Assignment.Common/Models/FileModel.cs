using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Common.Models
{
    public class FileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public int TotalRecords { get; set; }
        public DateTime CreatedDate { get; set; }

        public FileModel() { }
        public FileModel(string name, string fileType, int fileSize, int totalRecords, DateTime createdDate)
        {
            Name = name;
            FileType = fileType;
            FileSize = fileSize;
            TotalRecords = totalRecords;
            CreatedDate = createdDate;
        }

    }

    public class FileDetailModel
    {
        public int Id { get; set; }
        public string FileId { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }

    }

    public class UploadFileModel
    {
        public string TransactionId { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string TransactionDate { get; set; }
        public string Status { get; set; }

    }
}
