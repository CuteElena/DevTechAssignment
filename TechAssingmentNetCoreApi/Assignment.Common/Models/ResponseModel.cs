using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment.Common.Models
{
    public class ResponseModel
    {
        public string RespCode { get; set; }
        public string RespDescription { get; set; }

        public ResponseModel() { }
        public ResponseModel(string respCode, string respDescription)
        {
            RespCode = respCode;
            RespDescription = respDescription;
        }
    }
}