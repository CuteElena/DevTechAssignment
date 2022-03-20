using TechAssignmentWebApp.Areas.ManageFile.Models;
using TechAssignmentWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechAssignmentWebApp;

namespace TechAssignmentWebApp.Areas.ManageFile.Controllers
{
    public class FileController : BaseController
    {

        public ActionResult Index()
        {
            return View("UploadFile");
        }


        public ActionResult Report(int fileId)
        {
            ViewData["FileId"] = fileId;
            return View("Report");

        }


        [HttpPost]
        public ActionResult UploadFiles(UploadFileModel uploadFileModel)
        {
            var fileTypes = new[] { ".CSV", ".XML" };
            if (uploadFileModel.Files.Any(x => !fileTypes.Any(y => y.Equals(Path.GetExtension(x.FileName.ToUpper()))))) return Json(new { success = false, message = "Unknown format" }, JsonRequestBehavior.AllowGet);

            if (!ModelState.IsValid) return Json(new { success = false, message = "Validation failed." }, JsonRequestBehavior.AllowGet);

            var files = uploadFileModel.Files;
            if (files == null) return Json(new { success = false, message = "Validation failed." }, JsonRequestBehavior.AllowGet);

            
            foreach (var file in files)
            {
                if (file == null) continue;
                var fileBytes = new BinaryReader(file.InputStream).ReadBytes(file.ContentLength);

                try
                {
                    
                    string extension = Path.GetExtension(files[0].FileName);

                    var reqModel = new FileUploadRequestModel
                    {
                        FileName = Path.GetFileNameWithoutExtension(files[0].FileName),
                        FileSize = file.ContentLength,
                        FileContent = fileBytes,
                        FileType = extension
                    };

                    var httpResponse = apiRepository.UploadFile("API/UploadFile", reqModel).Result;
                    httpResponse.EnsureSuccessStatusCode();
                    var response = httpResponse.Content.ReadAsStringAsync().Result;
                    var fileResponse = JsonConvert.DeserializeObject<FileUploadResponseModel>(response);

                    ViewBag.UploadStatus = files.Count() + fileResponse.RespDescription;
                    ViewBag.UploadStatus = fileResponse.RespDescription + "(" + fileResponse.TotalRecords + ")";
                    if (fileResponse.RespCode != "000")
                    {
                        return Json(new { success = false, message = fileResponse.RespDescription }, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

           
            return Json(new { success = true, message = "files uploaded successfully." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAllFiles(int draw, int start, int length)
        {
            var httpResponse = apiRepository.GetAllUploadFile("API/GetAllUploadFiles").Result;
            httpResponse.EnsureSuccessStatusCode();
            var response = httpResponse.Content.ReadAsStringAsync().Result;
            var fileResponse = JsonConvert.DeserializeObject<ReportFilterResponseModel>(response);
            if (fileResponse.RespCode != "000") return Json(new EmptyResult(), JsonRequestBehavior.AllowGet);

            var lstModel = new List<FileModel>();
            foreach(DataRow row in fileResponse.lstData.Rows)
            {
                var model = new FileModel();
                model.Id = Convert.ToInt32(row["Id"].ToString());
                model.Name = row["Name"].ToString();
                model.FileType = row["FileType"].ToString();
                model.FileSize = Convert.ToInt32(row["FileSize"].ToString());
                model.TotalRecords = Convert.ToInt32(row["TotalRecords"].ToString());
                model.CreatedDate = Convert.ToDateTime(row["CreatedDate"].ToString());

                lstModel.Add(model);
            }
          
            var result = new
            {
                draw = draw,
                recordsTotal = lstModel.Count,
                recordsFiltered = lstModel.Count,
                data = lstModel
            };


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAllFileDetail(int fileId, int draw, int start, int length)
        {
            var httpResponse = apiRepository.GetAllUploadFileDetailByFileId("API/GeAllFileDetailsByFile", fileId).Result;
            httpResponse.EnsureSuccessStatusCode();
            var response = httpResponse.Content.ReadAsStringAsync().Result;
            var fileResponse = JsonConvert.DeserializeObject<ReportFilterResponseModel>(response);
            if (fileResponse.RespCode != "000") return Json(new { draw = draw, recordsTotal = 0, recordsFiltered = 0, data = "" }, JsonRequestBehavior.AllowGet);

            var lstModel = new List<FileDetailModel>();
            foreach (DataRow row in fileResponse.lstData.Rows)
            {
                var model = new FileDetailModel();
                model.Id = Convert.ToInt32(row["Id"].ToString());
                model.FileName = row["FileName"].ToString();
                model.FileId = Convert.ToInt32(row["FileId"].ToString());
                model.TransactionId = row["TransactionId"].ToString();
                model.Amount = Convert.ToDecimal(row["Amount"].ToString());
                model.Currency = row["Currency"].ToString();
                model.Status = row["Status"].ToString();
                model.TransactionDate = Convert.ToDateTime(row["TransactionDate"].ToString());

                lstModel.Add(model);
            }

            var result = new
            {
                draw = draw,
                recordsTotal = lstModel.Count,
                recordsFiltered = lstModel.Count,
                data = lstModel
            };


            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}