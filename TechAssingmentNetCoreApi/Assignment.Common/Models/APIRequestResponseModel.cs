using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Assignment.Common.Models
{
    public class APIRequestResponseModel
    {
    }

    public class FileDetailRequestModel
    {
        public int FileId { get; set; }

    }

    public class FileUploadRequestModel
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public byte[] FileContent { get; set; }
    }

    public class FileUploadResponseModel : ResponseModel
    {
        public int TotalRecords { get; set; }

    }

    public class FileSaveResponseModel : ResponseModel
    {
        public int FileId { get; set; }

    }

    public class ReportFilterModel
    {
        public string Currency { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class ReportFilterResponseModel : ResponseModel
    {
        public DataTable lstData { get; set; }

    }

}