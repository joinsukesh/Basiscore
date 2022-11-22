
namespace Basiscore.ContentAuthoringGuide.Areas.ContentAuthoringGuide.Utilities
{
    using Sitecore.Configuration;
    using System;   
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Web;

    public class LogManager
    {
        #region VARIABLES
        /// <summary>
        /// LogModes define how a log should be saved (to a text file, to a table or both)
        /// </summary>
        enum LogModes
        {
            LogToFile = 1,
            LogToTable = 2,
            LogToFileAndTable = 3
        }

        private static string moduleName = Settings.GetSetting("ModuleName");
        private static string LogsFolderPath = HttpContext.Current.Server.MapPath("~" + Settings.GetSetting("LogsFolderPath"));
        private static string LogMode = "1";// Convert.ToString(ConfigurationManager.AppSettings["LogMode"]);

        /// <summary>
        /// LogTypes define if a log is an error or a message to be displyed to the user
        /// </summary>
        public enum LogTypes
        {
            Error = 1,
            Message = 2
        }

        #endregion

        #region METHODS

        public LogManager() { }

        /// <summary>
        /// save a log 
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="methodName"></param>
        /// <param name="objException"></param>    
        /// <param name="logType"></param>
        /// <param name="nonErrorMessage"></param>
        public static void SaveLog(string methodName, Exception objException, LogTypes logType, string nonErrorMessage)
        {
            //set default log mode if not set in config
            LogModes SelectedLogMode;
            Enum.TryParse(LogMode, out SelectedLogMode);

            switch (SelectedLogMode)
            {
                case LogModes.LogToTable:
                    LogToTable(moduleName, methodName, objException, logType, nonErrorMessage);
                    break;
                case LogModes.LogToFileAndTable:
                    LogToTextFile(moduleName, methodName, objException, logType, nonErrorMessage);
                    LogToTable(moduleName, methodName, objException, logType, nonErrorMessage);
                    break;
                case LogModes.LogToFile:
                default:
                    LogToTextFile(moduleName, methodName, objException, logType, nonErrorMessage);
                    break;
            }
        }

        /// <summary>
        /// insert log to database table
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="methodName"></param>
        /// <param name="objException"></param>
        ///<param name="logType"></param>
        /// <param name="nonErrorMessage"></param>    
        private static void LogToTable(string moduleName, string methodName, Exception objException, LogTypes logType, string nonErrorMessage)
        {
            SqlConnection connection = null;
            try
            {
                //using (connection = new SqlConnection(DBConnection.ConnectionString))
                //{
                //    SqlCommand cmd = new SqlCommand("usp_InsertLog");
                //    cmd.CommandType = CommandType.StoredProcedure;
                //    cmd.Connection = connection;
                //    cmd.Parameters.AddWithValue("@ModuleName", moduleName);
                //    cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);
                //    cmd.Parameters.AddWithValue("@MethodName", methodName);
                //    cmd.Parameters.AddWithValue("@ActualURL", Convert.ToString(System.Web.HttpContext.Current.Request.RawUrl));
                //    cmd.Parameters.AddWithValue("@RewriteURL", Convert.ToString(System.Web.HttpContext.Current.Request.Url));
                //    cmd.Parameters.AddWithValue("@ErrorType", objException != null ? Convert.ToString(objException.GetType()) : string.Empty);
                //    cmd.Parameters.AddWithValue("@ErrorMessage", objException != null ? Convert.ToString(objException.Message) : string.Empty);
                //    cmd.Parameters.AddWithValue("@InnerExceptionMessage", objException != null ? Convert.ToString(objException.InnerException) : string.Empty);
                //    cmd.Parameters.AddWithValue("@StackTrace", objException != null ? Convert.ToString(objException.StackTrace) : string.Empty);
                //    cmd.Parameters.AddWithValue("@LogType", Convert.ToString((LogTypes)Convert.ToInt32(logType)));
                //    cmd.Parameters.AddWithValue("@NonErrorMessage", nonErrorMessage);
                //    cmd.Parameters.AddWithValue("@EndTime", DateTime.Now);
                //    connection.Open();
                //    cmd.ExecuteNonQuery();
                //    connection.Close();
                //}
            }
            catch (Exception)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// insert log to a text file
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="methodName"></param>
        /// <param name="objException"></param>
        /// <param name="nonErrorMessage"></param>
        private static void LogToTextFile(string moduleName, string methodName, Exception objException, LogTypes logType, string nonErrorMessage)
        {
            //check if the log is an error or a message and save accordingly
            string LogFileName = string.Empty;
            string LogFilePath = string.Empty;

            try
            {
                //Create directory
                if (!Directory.Exists(LogsFolderPath))
                {
                    Directory.CreateDirectory(LogsFolderPath);
                }

                LogsFolderPath = LogsFolderPath.EndsWith("\\") ? LogsFolderPath : LogsFolderPath + "\\";

                switch (logType)
                {
                    case LogTypes.Error:
                        LogFileName = moduleName.EndsWith("_") ? moduleName + "ERR-" : moduleName + "_" + "ERR-";
                        break;
                    case LogTypes.Message:
                        LogFileName = moduleName.EndsWith("_") ? moduleName + "MSG-" : moduleName + "_" + "MSG-";
                        break;
                }

                LogFilePath = LogsFolderPath + LogFileName + DateTime.Today.Year + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day + ".txt";

                //Create log file
                if (!File.Exists(LogFilePath))
                {
                    File.Create(LogFilePath).Close();
                }

                //Append text to the log file                  
                using (StreamWriter oStreamWriter = File.AppendText(LogFilePath))
                {
                    oStreamWriter.WriteLine("===========================================================================");
                    oStreamWriter.WriteLine("START TIME              : {0}", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                    oStreamWriter.WriteLine("MODULE NAME             : {0}", moduleName);
                    oStreamWriter.WriteLine("METHOD NAME             : {0}", methodName);
                    oStreamWriter.WriteLine("ACTUAL URL              : {0}", Convert.ToString(System.Web.HttpContext.Current.Request.RawUrl));
                    oStreamWriter.WriteLine("REWRITE URL             : {0}", Convert.ToString(System.Web.HttpContext.Current.Request.Url));

                    switch (logType)
                    {
                        case LogTypes.Error:
                            oStreamWriter.WriteLine("ERROR TYPE              : {0}", objException.GetType());
                            oStreamWriter.WriteLine("ERROR MESSAGE           : {0}", objException.Message);
                            oStreamWriter.WriteLine("INNER EXCEPTION MESSAGE : {0}", objException.InnerException != null ? objException.InnerException.Message : string.Empty);
                            oStreamWriter.WriteLine("STACK TRACE             : ");
                            oStreamWriter.WriteLine(objException.StackTrace);
                            break;
                        case LogTypes.Message:
                            oStreamWriter.WriteLine("MESSAGE                 : {0}", nonErrorMessage);
                            break;
                    }

                    oStreamWriter.WriteLine("END TIME                : {0}", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                    oStreamWriter.Flush();
                    oStreamWriter.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region SQL SCRIPTS
        //----------------------sql query to create a table in DB to save logs--------------------------
        //    CREATE TABLE [dbo].[CsatFeedbackLogs](
        //    [LogID] [bigint] IDENTITY(1,1) NOT NULL,
        //    [ModuleName] [varchar](500) NULL,
        //    [StartTime] [datetime] NULL,
        //    [MethodName] [varchar](500) NULL,
        //    [ActualURL] [varchar](2000) NULL,
        //    [RewriteURL] [varchar](2000) NULL,
        //    [ErrorType] [varchar](1000) NULL,
        //    [ErrorMessage] [varchar](MAX) NULL,
        //    [InnerExceptionMessage] [varchar](MAX) NULL,
        //    [StackTrace] [text] NULL,
        //    [LogType] [varchar](50) NULL,
        //    [NonErrorMessage] [varchar](MAX) NULL,
        //    [EndTime] [datetime] NULL    
        // CONSTRAINT [PK_LogID] PRIMARY KEY CLUSTERED 
        //(
        //    [LogID] ASC
        //)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
        //) ON [PRIMARY]

        //-----------------------stored procedure to save logs in table---------------------
        //    CREATE PROC usp_InsertLog
        //(
        //    @ModuleName varchar(500),
        //    @StartTime datetime,	
        //    @MethodName varchar(500),
        //    @ActualURL varchar(2000),
        //    @RewriteURL varchar(2000),
        //    @ErrorType varchar(1000),
        //    @ErrorMessage varchar(MAX),
        //    @InnerExceptionMessage varchar(MAX),
        //    @StackTrace text,
        //    @LogType varchar(50),
        //    @NonErrorMessage varchar(MAX),
        //    @EndTime datetime    
        //)
        //AS
        //BEGIN
        //    INSERT INTO CsatFeedbackLogs( 
        //        ModuleName,
        //        StartTime,
        //        MethodName,
        //        ActualURL,
        //        RewriteURL,
        //        ErrorType,
        //        ErrorMessage,
        //        InnerExceptionMessage,
        //        StackTrace,
        //        LogType,
        //        NonErrorMessage,
        //        EndTime    
        //    )
        //    VALUES(
        //        @ModuleName,
        //        @StartTime,
        //        @MethodName,
        //        @ActualURL,
        //        @RewriteURL,
        //        @ErrorType,
        //        @ErrorMessage,
        //        @InnerExceptionMessage,
        //        @StackTrace,
        //        @LogType,
        //        @NonErrorMessage,
        //        @EndTime    
        //    )
        //END
        #endregion

        #region HOW TO USE
        //To Log an Error
        //LogManager.SaveLog(GlobalConstants.MODULE_NAME, System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, ex, LogManager.LogTypes.Error, string.Empty);

        //To Log a Message
        //LogManager.SaveLog(GlobalConstants.MODULE_NAME, System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, null, LogManager.LogTypes.Message, "TestMessage");
        #endregion
    }
}