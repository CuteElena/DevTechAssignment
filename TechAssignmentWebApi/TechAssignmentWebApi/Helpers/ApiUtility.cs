using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using TechAssignmentWebApi.Domain.Models;
using TechAssignmentWebApi.Models;

namespace TechAssignmentWebApi.Helpers
{
    public class ApiUtility
    {
        public const string ERROR_MESSAGE = "System Error";

        public static DataTable ConvertCSVToDataTable(byte[] fileContent)
        {
            var table = new DataTable();
            var splitChars = new char[] { ',', '|' };

            using (var stream = new MemoryStream(fileContent))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    // get the first row of csv
                    string header = reader.ReadLine();
                    var fields = header.Split(splitChars);

                    foreach (string column in fields)
                    {
                        // add columns to new datatable based on first row of csv
                        var col = column.Replace(" ", string.Empty);
                        table.Columns.Add(col.Trim());
                    }

                    string row = reader.ReadLine();
                    // read to end
                    while (row != null)
                    {
                        // add each row to datatable 
                        var rowDatas = row.Split(splitChars);

                        foreach (var data in rowDatas)
                        {
                            if (string.IsNullOrWhiteSpace(data)) return new DataTable();

                        }
                        table.Rows.Add(rowDatas);
                        row = reader.ReadLine();
                    }
                }
            }

            return table;
        }
        public static DataTable ConvertXMLToDataTable(byte[] fileContent)
        {
            var data = Encoding.UTF8.GetString(fileContent);

            DataSet ds = new DataSet("tbl");
            ds.ReadXml(new StringReader(data));

            var table = new DataTable();
            table.Columns.Add("TransactionId", typeof(string));
            table.Columns.Add("Amount", typeof(string));
            table.Columns.Add("Currency", typeof(string));
            table.Columns.Add("TransactionDate", typeof(string));
            table.Columns.Add("Status", typeof(string));

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var model = new UploadFileModel();
                    model.TransactionDate = row["TransactionDate"].ToString();
                    model.Status = row["Status"].ToString();
                    model.TransactionId = row["id"].ToString();

                    if (ds.Tables.Count > 1)
                    {
                        foreach (DataRow pRow in ds.Tables[1].Rows)
                        {
                            model.Amount = pRow["Amount"].ToString();
                            model.Currency = pRow["CurrencyCode"].ToString();
                        }
                    }

                    if (string.IsNullOrEmpty(model.TransactionId) || string.IsNullOrEmpty(model.Amount) || string.IsNullOrEmpty(model.Currency)
                        || string.IsNullOrEmpty(model.TransactionDate) || string.IsNullOrEmpty(model.Status)) return new DataTable();

                    table.Rows.Add(model.TransactionId, model.Amount, model.Currency, model.TransactionDate, model.Status);


                }
            }


            return table;
        }
        public static bool CheckNullorEmptyDataTable(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                return true;

            }
            else
            {
                return false;
            }
        }
        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        public static ResponseModel CheckDataValue(List<UploadFileModel> lst, string fileType)
        {
            foreach (var data in lst)
            {
                decimal amount;
                if (!Decimal.TryParse(data.Amount, out amount))
                {
                    string errMsg = "[" + data + "] Amount Field wrong at row " + lst.IndexOf(data) + "";
                    return new ResponseModel("015", errMsg);

                }

                DateTime txnDate;
                if (!DateTime.TryParse(data.TransactionDate, out txnDate))
                {
                    string format = "dd/MM/yyyy hh:mm:ss";
                    DateTime dateTime;
                    if (!DateTime.TryParseExact(data.TransactionDate, format, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out dateTime))
                    {
                        string errMsg = "[" + data + "] TransactionDate Field wrong at row " + lst.IndexOf(data) + "";
                        return new ResponseModel("016", errMsg);
                    }

                }

                if (fileType == ".csv")
                {
                    if (!(data.Status.Trim() == "Approved" || data.Status.Trim() == "Failed" || data.Status.Trim() == "Finished"))
                        return new ResponseModel("017", "Invalid Status");

                }
                else if (fileType == ".xml")
                {
                    if (!(data.Status.Trim() == "Approved" || data.Status.Trim() == "Rejected" || data.Status.Trim() == "Done"))
                        return new ResponseModel("017", "Invalid Status");

                }
            }

            return new ResponseModel("000", "Success");
        }
        public static string ExceptionLineAndFile(Exception ex)
        {
            var st = new StackTrace(ex, true);
            var frame = st.GetFrame(0);
            var line = frame.GetFileLineNumber();

            return frame.GetFileName() + ": Line No. " + frame.GetFileLineNumber();
        }
        public HttpResponseMessage GetAPIExceptionResponse(Exception ex)
        {
            var responseModel = new ResponseModel();
            responseModel.RespCode = "099";
            responseModel.RespDescription = ERROR_MESSAGE;

            var response = new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(responseModel))
            };

            return response;

        }
        public HttpResponseMessage GetHttpAPIResponse(string RespCode, string RespDescription)
        {
            var model = new ResponseModel();

            model.RespCode = RespCode;
            model.RespDescription = RespDescription;

            var response = new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(model))
            };

            return response;
        }
    }
}