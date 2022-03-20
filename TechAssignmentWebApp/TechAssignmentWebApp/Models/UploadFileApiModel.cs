using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TechAssignmentWebApp.Models
{

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

    public class ReportFilterResponseModel : ResponseModel
    {
        public DataTable lstData { get; set; }
    }

}