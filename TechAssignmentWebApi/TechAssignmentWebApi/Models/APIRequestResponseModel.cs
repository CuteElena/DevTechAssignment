using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TechAssignmentWebApi.Models
{
    public class APIRequestResponseModel
    {
    }

    public class FileUploadRequestModel
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public byte[] FileContent { get; set; }
    }

    public class FileSaveResponseModel : ResponseModel
    {
        public int FileId { get; set; }

    }

   

}