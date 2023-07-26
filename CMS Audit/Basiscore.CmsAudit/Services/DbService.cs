namespace Basiscore.CmsAudit.Services
{
    using Basiscore.CmsAudit.Models;
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    public class DbService
    {
        private SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Configurations.CoreDbName].ConnectionString);

        public void InsertItemAuditLog(AuditLog_Item ali)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_Basiscore_CmsAudit_InsertItemAuditLog", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ItemId", SqlDbType.UniqueIdentifier).Value = ali.ItemId;
                cmd.Parameters.AddWithValue("@ItemName", SqlDbType.VarChar).Value = ali.ItemName;
                cmd.Parameters.AddWithValue("@ItemPath", SqlDbType.VarChar).Value = ali.ItemPath;
                cmd.Parameters.AddWithValue("@TemplateId", SqlDbType.UniqueIdentifier).Value = ali.TemplateId;
                cmd.Parameters.AddWithValue("@ItemLanguage", SqlDbType.VarChar).Value = ali.ItemLanguage;
                cmd.Parameters.AddWithValue("@ItemVersion", SqlDbType.Int).Value = ali.ItemVersion;
                cmd.Parameters.AddWithValue("@Event", SqlDbType.VarChar).Value = ali.Event;
                cmd.Parameters.AddWithValue("@ActionedBy", SqlDbType.VarChar).Value = ali.ActionedBy;
                cmd.Parameters.AddWithValue("@ItemDataBeforeSave", SqlDbType.NVarChar).Value = ali.ItemDataBeforeSave;
                cmd.Parameters.AddWithValue("@ItemDataAfterSave", SqlDbType.NVarChar).Value = ali.ItemDataAfterSave;
                cmd.Parameters.AddWithValue("@Comments", SqlDbType.NVarChar).Value = ali.Comments;
                cmd.Parameters.AddWithValue("@LoggedTime", SqlDbType.DateTime).Value = ali.LoggedTime;
                int rowsInserted = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                con.Close();
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable GetItemAuditLogs(ItemAuditLogRequest itemAuditLogRequest)
        {
            DataTable dtData = null;
            DataSet dsResult = new DataSet();

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_Basiscore_CmsAudit_GetItemLogs", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@GetOnlySitePublishLogs", SqlDbType.Bit).Value = itemAuditLogRequest.GetOnlySitePublishLogs;
                cmd.Parameters.AddWithValue("@GetOnlyItemPublishLogs", SqlDbType.Bit).Value = itemAuditLogRequest.GetOnlyItemPublishLogs;
                cmd.Parameters.AddWithValue("@GetOnlyPublishLogs", SqlDbType.VarChar).Value = itemAuditLogRequest.GetOnlyPublishLogs;
                cmd.Parameters.AddWithValue("@ItemId", SqlDbType.NVarChar).Value = itemAuditLogRequest.ItemId;
                cmd.Parameters.AddWithValue("@ActionedBy", SqlDbType.VarChar).Value = itemAuditLogRequest.ActionedBy;
                cmd.Parameters.AddWithValue("@ItemLanguage", SqlDbType.VarChar).Value = itemAuditLogRequest.ItemLanguage;
                cmd.Parameters.AddWithValue("@FromDate", SqlDbType.DateTime).Value = itemAuditLogRequest.FromDate;
                cmd.Parameters.AddWithValue("@ToDate", SqlDbType.DateTime).Value = itemAuditLogRequest.ToDate;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsResult);

                if (dsResult != null && dsResult.Tables != null && dsResult.Tables.Count > 0 &&
                dsResult.Tables[0].Rows != null && dsResult.Tables[0].Rows.Count > 0)
                {
                    dtData = dsResult.Tables[0];
                }
            }
            catch (Exception ex)
            {
                con.Close();
                throw ex;
            }
            finally
            {
                con.Close();
            }

            return dtData;
        }

        public void InsertAccountAuditLog(AuditLog_Account ala)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_Basiscore_CmsAudit_InsertAccountAuditLog", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", SqlDbType.UniqueIdentifier).Value = ala.UserId;
                cmd.Parameters.AddWithValue("@Username", SqlDbType.VarChar).Value = ala.Username;
                cmd.Parameters.AddWithValue("@FullName", SqlDbType.NVarChar).Value = ala.FullName;
                cmd.Parameters.AddWithValue("@Email", SqlDbType.NVarChar).Value = ala.Email;
                cmd.Parameters.AddWithValue("@Domain", SqlDbType.VarChar).Value = ala.Domain;
                cmd.Parameters.AddWithValue("@IsAdministrator", SqlDbType.Bit).Value = ala.IsAdministrator;
                cmd.Parameters.AddWithValue("@Roles", SqlDbType.NVarChar).Value = ala.Roles;
                cmd.Parameters.AddWithValue("@Event", SqlDbType.VarChar).Value = ala.Event;
                cmd.Parameters.AddWithValue("@ActionedBy", SqlDbType.VarChar).Value = ala.ActionedBy;
                cmd.Parameters.AddWithValue("@Comments", SqlDbType.NVarChar).Value = ala.Comments;
                cmd.Parameters.AddWithValue("@LoggedTime", SqlDbType.DateTime).Value = ala.LoggedTime;
                int rowsInserted = cmd.ExecuteNonQuery();                
            }
            catch (Exception ex)
            {
                con.Close();
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable GetItemAuditDataSummary()
        {
            DataTable dtData = null;
            DataSet dsResult = new DataSet();

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_Basiscore_CmsAudit_GetItemAuditDataSummary", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsResult);

                if (dsResult != null && dsResult.Tables != null && dsResult.Tables.Count > 0 &&
                dsResult.Tables[0].Rows != null && dsResult.Tables[0].Rows.Count > 0)
                {
                    dtData = dsResult.Tables[0];
                }
            }
            catch (Exception ex)
            {
                con.Close();
                throw ex;
            }
            finally
            {
                con.Close();
            }

            return dtData;
        }

        public int DeleteItemAuditLogs(DateTime fromDate, DateTime toDate)
        {
            int deletedRows = 0;

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_Basiscore_CmsAudit_DeleteItemAuditLogs", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromDate", SqlDbType.DateTime).Value = fromDate;
                cmd.Parameters.AddWithValue("@ToDate", SqlDbType.DateTime).Value = toDate;
                deletedRows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                con.Close();
                throw ex;
            }
            finally
            {
                con.Close();
            }

            return deletedRows;
        }
    }
}