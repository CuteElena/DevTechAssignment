using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TechAssignmentWebApp.Areas.ManageFile.Models
{
    public class FileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public int TotalRecords { get; set; }
        public DateTime CreatedDate { get; set; }


    }

    public class FileDetailModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public int FileId { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }

    }

    public class UploadFileModel
    {
        [Required(ErrorMessage = "Please select file.")]
        [Display(Name = "Browse File")]
        public HttpPostedFileBase[] Files { get; set; }

       

    }
}