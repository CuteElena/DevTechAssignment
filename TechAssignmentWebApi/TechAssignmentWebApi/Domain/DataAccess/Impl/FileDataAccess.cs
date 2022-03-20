using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TechAssignmentWebApi.Domain.Models;
using TechAssignmentWebApi.Models;

namespace TechAssignmentWebApi.Domain.DataAccess.Impl
{
    public class FileDataAccess : IFileDataAccess
    {
        protected readonly string strConnection;
        MySqlTransaction transaction;
        public bool useTransaction = false;
        MySqlConnection Connection = new MySqlConnection();
        public FileDataAccess()
        {
            strConnection = ConfigurationManager.ConnectionStrings["DBConnection"].ToString();
            Connection = new MySqlConnection(strConnection);
        }

        MySqlTransaction Transaction
        {
            get { return this.transaction; }

            set { this.transaction = value; }
        }

        public async Task<FileSaveResponseModel> SaveBulkFile(FileModel file)
        {
            MySqlCommand cmd = null;
            var resModel = new FileSaveResponseModel();

            try
            {
                string query = "INSERT INTO UploadFile ( Name , FileType , FileSize, TotalRecords, CreatedDate) "
                          + " VALUES(@Name , @FileType , @FileSize, @TotalRecords, @CreatedDate); ";

                using (MySqlConnection conn = new MySqlConnection(strConnection))
                {
                    await conn.OpenAsync();
                    var affectedRows = 0;
                    cmd = new MySqlCommand();
                    cmd = conn.CreateCommand();
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@Name", file.Name);
                    cmd.Parameters.AddWithValue("@FileType", file.FileType);
                    cmd.Parameters.AddWithValue("@FileSize", file.FileSize);
                    cmd.Parameters.AddWithValue("@TotalRecords", file.TotalRecords);
                    cmd.Parameters.AddWithValue("@CreatedDate", file.CreatedDate);

                    affectedRows = await cmd.ExecuteNonQueryAsync();
                    if (affectedRows < 1)
                    {
                        resModel.RespCode = "017";
                        resModel.RespDescription = "Fail to save File";
                        return resModel;
                    }

                    resModel.FileId = (int)cmd.LastInsertedId;
                    resModel.RespCode = "000";
                    resModel.RespDescription = "Success";

                    conn.Close();
                    return resModel;
                }

            }
            catch (Exception ex)
            {
                if (transaction != null) transaction.Rollback();
                resModel.RespCode = "017";
                resModel.RespDescription = "Fail to Save File [" + ex.Message + "]";
                return resModel;
            }
            finally
            {
                cmd?.Connection.Close();
            }
        }

        public async Task<ResponseModel> SaveUploadFileDeatail(List<UploadFileModel> fileRecords, int fileId)
        {
            var current = new UploadFileModel();
            MySqlTransaction transaction = null;
            MySqlCommand cmd = null;
            var resModel = new ResponseModel();

            string query = "INSERT INTO UploadFileDetail (FileId, TransactionId ,  Amount , Currency , TransactionDate, Status) "
              + " VALUES(@FileId, @TransactionId ,@Amount , @Currency ,@TransactionDate,@Status); ";

            using (MySqlConnection conn = new MySqlConnection(strConnection))
            {
                try
                {

                    await conn.OpenAsync();

                    transaction = conn.BeginTransaction();

                    foreach (var fileDetail in fileRecords)
                    {
                        current = fileDetail;
                        cmd = new MySqlCommand();
                        cmd = conn.CreateCommand();
                        cmd.CommandText = query;

                        string format = "dd/MM/yyyy h:mm:ss";
                        var transactionDate = new DateTime();
                        DateTime txnDate;
                        if (DateTime.TryParseExact(fileDetail.TransactionDate, format, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out txnDate)) transactionDate = txnDate;
                        else transactionDate = DateTime.Parse(fileDetail.TransactionDate);
                        var status = fileDetail.Status;
                        if (!string.IsNullOrEmpty(status)) fileDetail.Status = status.Trim();

                        //string status = string.Empty;
                        //if (fileDetail.Status == "Approved") status = "A";
                        //if (fileDetail.Status == "Failed" || fileDetail.Status == "Rejected") status = "R";
                        //if (fileDetail.Status == "Finished" || fileDetail.Status == "Done") status = "D";

                        cmd.Parameters.AddWithValue("@FileId", fileId);
                        cmd.Parameters.AddWithValue("@TransactionId", fileDetail.TransactionId);
                        cmd.Parameters.AddWithValue("@Amount", fileDetail.Amount);
                        cmd.Parameters.AddWithValue("@Currency", fileDetail.Currency);
                        cmd.Parameters.AddWithValue("@TransactionDate", transactionDate);
                        cmd.Parameters.AddWithValue("@Status", fileDetail.Status);

                        var affectedRows = await cmd.ExecuteNonQueryAsync();
                    }

                    transaction.Commit();

                    resModel.RespCode = "000";
                    resModel.RespDescription = "Success";

                    conn.Close();
                    return resModel;
                }
                catch (Exception ex)
                {
                    if (transaction != null) transaction.Rollback();

                    int index = fileRecords.IndexOf(current);
                    resModel.RespCode = "018";
                    resModel.RespDescription = "Fail to save FileDetail at data record:" + index + " with error[" + ex.Message + "]";
                    //  throw ex;
                    return resModel;
                }
                finally
                {
                    cmd?.Connection.Close();


                }
            }

        }

        public async Task<DataTable> GetTransactionsByCurrencyFilter(string currency)
        {
            MySqlCommand cmd = null;
            var errorMesg = string.Empty;
            DataTable dt = new DataTable();

            string query = "SELECT TransactionId as Id ,concat(Amount ,' ',Currency) as Payment, "
                         + " CASE "
                         + "     WHEN Status = 'Approved' THEN 'A' "
                         + "      WHEN Status = 'Rejected' OR Status = 'Failed' THEN 'R' "
                         + "     WHEN Status = 'Finished' OR Status = 'Done' THEN 'D' "
                         + "  END as Status "
                         + " FROM UploadFileDetail Where currency= @currency Order by Id";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(strConnection))
                {
                    await conn.OpenAsync();
                    cmd = new MySqlCommand();
                    cmd = conn.CreateCommand();
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@currency", currency);
                    var reader = await cmd.ExecuteReaderAsync();
                    dt.Load(reader);
                    reader.Close();
                    conn.Close();
                    return dt;

                }
            }

            catch (Exception ex)
            {
                errorMesg = ex.Message;
                throw ex;
            }
            finally
            {
                cmd?.Connection.Close();

            }
        }

        public async Task<DataTable> GetTransactionsByStatusFilter(string status)
        {
            MySqlCommand cmd = null;
            var errorMesg = string.Empty;
            DataTable dt = new DataTable();

            string query = "SELECT TransactionId as Id ,concat(Amount ,' ',Currency) as Payment, "
                          + " CASE "
                          + "     WHEN Status = 'Approved' THEN 'A' "
                          + "     WHEN Status = 'Rejected' OR Status = 'Failed' THEN 'R' "
                          + "     WHEN Status = 'Finished' OR Status = 'Done' THEN 'D' "
                          + "  END as Status "
                          + " FROM UploadFileDetail ";


            if (!string.IsNullOrEmpty(status))
            {
                if (status == "A") query += "Where Status = 'Approved'";
                if (status == "R") query += "Where Status = 'Failed' OR Status = 'Rejected'";
                if (status == "D") query += "Where Status = 'Finished' OR Status = 'Done'";
            }
            query += "  Order by Id ";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(strConnection))
                {
                    await conn.OpenAsync();
                    cmd = new MySqlCommand();
                    cmd = conn.CreateCommand();
                    cmd.CommandText = query;
                    var reader = await cmd.ExecuteReaderAsync();
                    dt.Load(reader);
                    reader.Close();
                    conn.Close();
                    return dt;

                }
            }

            catch (Exception ex)
            {
                errorMesg = ex.Message;
                throw ex;
            }
            finally
            {
                cmd?.Connection.Close();

            }
        }

        public async Task<DataTable> GetTransactionsByDateFilter(DateTime startDate, DateTime endDate)
        {
            MySqlCommand cmd = null;
            var errorMesg = string.Empty;
            DataTable dt = new DataTable();

            string query = "SELECT TransactionId as Id ,concat(Amount ,' ',Currency) as Payment, "
                           + " CASE "
                           + "     WHEN Status = 'Approved' THEN 'A' "
                           + "      WHEN Status = 'Rejected' OR Status = 'Failed' THEN 'R' "
                           + "     WHEN Status = 'Finished' OR Status = 'Done' THEN 'D' "
                           + "  END as Status "
                           + " FROM UploadFileDetail Where TransactionDate BETWEEN @startDate AND @endDate Order by Id";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(strConnection))
                {
                    await conn.OpenAsync();
                    cmd = new MySqlCommand();
                    cmd = conn.CreateCommand();
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    var reader = await cmd.ExecuteReaderAsync();
                    dt.Load(reader);
                    reader.Close();
                    conn.Close();
                    return dt;

                }
            }

            catch (Exception ex)
            {
                errorMesg = ex.Message;
                throw ex;
            }
            finally
            {
                cmd?.Connection.Close();

            }
        }

        public async Task<DataTable> GetAllUploadFile()
        {
            MySqlCommand cmd = null;
            var errorMesg = string.Empty;
            DataTable dt = new DataTable();

            string query = "select * from UploadFile  order by Id desc ; ";


            try
            {
                using (MySqlConnection conn = new MySqlConnection(strConnection))
                {
                    await conn.OpenAsync();
                    cmd = new MySqlCommand();
                    cmd = conn.CreateCommand();
                    cmd.CommandText = query;
                    var reader = await cmd.ExecuteReaderAsync();
                    dt.Load(reader);
                    reader.Close();
                    conn.Close();
                    return dt;

                }
            }

            catch (Exception ex)
            {
                errorMesg = ex.Message;
                throw ex;
            }
            finally
            {
                cmd?.Connection.Close();

            }
        }

        public async Task<DataTable> GetAllUploadFileDetailByFileId(int fileId)
        {
            MySqlCommand cmd = null;
            var errorMesg = string.Empty;
            DataTable dt = new DataTable();

            string query = @"select f.Name as FileName , d.* from UploadFileDetail d inner join UploadFile f 
                    on d.FileId = f.Id Where FileId = @FileId order by Id desc";


            try
            {
                using (MySqlConnection conn = new MySqlConnection(strConnection))
                {
                    await conn.OpenAsync();
                    cmd = new MySqlCommand();
                    cmd = conn.CreateCommand();
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@FileId", fileId);
                    var reader = await cmd.ExecuteReaderAsync();                 
                    dt.Load(reader);
                    reader.Close();
                    conn.Close();
                    return dt;

                }
            }

            catch (Exception ex)
            {
                errorMesg = ex.Message;
                throw ex;
            }
            finally
            {
                cmd?.Connection.Close();

            }
        }

        public async Task<DataTable> GeUploadFileById(int fileId)
        {
            MySqlCommand cmd = null;
            var errorMesg = string.Empty;
            DataTable dt = new DataTable();

            string query = "select Id from UploadFile Where Id = @Id;";


            try
            {
                using (MySqlConnection conn = new MySqlConnection(strConnection))
                {
                    await conn.OpenAsync();
                    cmd = new MySqlCommand();
                    cmd = conn.CreateCommand();
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@Id", fileId);
                    var reader = await cmd.ExecuteReaderAsync();                   
                    dt.Load(reader);
                    reader.Close();
                    conn.Close();
                    return dt;

                }
            }

            catch (Exception ex)
            {
                errorMesg = ex.Message;
                throw ex;
            }
            finally
            {
                cmd?.Connection.Close();

            }
        }
    }
}