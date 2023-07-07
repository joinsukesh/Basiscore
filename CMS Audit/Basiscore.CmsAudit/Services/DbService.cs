namespace Basiscore.CmsAudit.Services
{
    using Basiscore.CmsAudit.Models;
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    public class DbService
    {
        public void InsertCmsItemAuditLog(AuditLog_Item ciai)
        {
            //DataTable dtData = null;
            //DataSet dsResult = new DataSet();
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Configurations.CoreDbName].ConnectionString);

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_Basiscore_InsertCmsItemAuditLog", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ItemId", SqlDbType.UniqueIdentifier).Value = ciai.ItemId;
                cmd.Parameters.AddWithValue("@ItemName", SqlDbType.VarChar).Value = ciai.ItemName;
                cmd.Parameters.AddWithValue("@ItemPath", SqlDbType.VarChar).Value = ciai.ItemPath;
                cmd.Parameters.AddWithValue("@TemplateId", SqlDbType.UniqueIdentifier).Value = ciai.TemplateId;
                cmd.Parameters.AddWithValue("@ItemLanguage", SqlDbType.VarChar).Value = ciai.ItemLanguage;
                cmd.Parameters.AddWithValue("@ItemVersion", SqlDbType.Int).Value = ciai.ItemVersion;
                cmd.Parameters.AddWithValue("@Event", SqlDbType.VarChar).Value = ciai.Event;
                cmd.Parameters.AddWithValue("@ActionedBy", SqlDbType.VarChar).Value = ciai.ActionedBy;
                cmd.Parameters.AddWithValue("@ItemDataBeforeSave", SqlDbType.NVarChar).Value = ciai.ItemDataBeforeSave;
                cmd.Parameters.AddWithValue("@ItemDataAfterSave", SqlDbType.NVarChar).Value = ciai.ItemDataAfterSave;
                cmd.Parameters.AddWithValue("@Comments", SqlDbType.NVarChar).Value = ciai.Comments;
                cmd.Parameters.AddWithValue("@LoggedTime", SqlDbType.DateTime).Value = ciai.LoggedTime;
                int rowsInserted = cmd.ExecuteNonQuery();
                //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //adapter.Fill(dsResult);

                //if (dsResult != null && dsResult.Tables != null && dsResult.Tables.Count > 0 &&
                //dsResult.Tables[0].Rows != null && dsResult.Tables[0].Rows.Count > 0)
                //{
                //    dtData = dsResult.Tables[0];
                //}
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

            //return dtData;
        }
    }
}