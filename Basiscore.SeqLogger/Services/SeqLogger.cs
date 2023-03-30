namespace Basiscore.SeqLogger.Services
{
    using log4net.Appender;
    using log4net.spi;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;

    public class SeqLogger : ForwardingAppender
    {
        /// <summary>
        /// The ApplicationName parameter from Basiscore.SeqLogger.config 
        /// </summary>
        private string _ApplicationName;
        public string ApplicationName
        {
            get => _ApplicationName;
            set => _ApplicationName = value;
        }

        /// <summary>
        /// The IncludeSystemLogs parameter from Basiscore.SeqLogger.config 
        /// </summary>
        private bool _IncludeSystemLogs;
        public bool IncludeSystemLogs
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(_IncludeSystemLogs);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            set => _IncludeSystemLogs = value;
        }

        /// <summary>
        /// The ExcludeLoggers parameter from Basiscore.SeqLogger.config 
        /// </summary>
        private string _ExcludeLoggers;
        public string ExcludeLoggers
        {
            get => _ExcludeLoggers;
            set => _ExcludeLoggers = value;
        }

        /// <summary>
        /// The SeqApiUrl parameter from Basiscore.SeqLogger.config 
        /// </summary>
        private string _SeqApiUrl;
        public string SeqApiUrl
        {
            get => _SeqApiUrl;
            set => _SeqApiUrl = value;
        }

        /// <summary>
        /// This method will be triggered after the default Sitecore .txt log is created.
        /// Here, the event will be posted to the Seq API
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                bool logThisEvent = LogThisEvent(loggingEvent);

                if (logThisEvent)
                {
                    HttpResponseMessage res = null;
                    string content = GetSeqClefJson(loggingEvent);
                    StringContent stringContent = new StringContent(content, Encoding.UTF8, "application/vnd.serilog.clef");

                    using (HttpClient client = new HttpClient())
                    {
                        res = client.PostAsync(new Uri(SeqApiUrl), stringContent).Result;

                        if (res != null)
                        {
                            var statusCode = res.StatusCode.ToString();
                            var responseMessage = res.Content != null ? res.Content.ReadAsStringAsync().Result : string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Determines if this event should be logged
        /// Check if the system events have to be logged. If not, then log only the appication specific events
        /// </summary>
        /// <param name="loggingEvent"></param>
        /// <returns></returns>
        private bool LogThisEvent(LoggingEvent loggingEvent)
        {
            bool logThisEvent = false;
            string logger = loggingEvent.LoggerName;

            if (IncludeSystemLogs)
            {
                logThisEvent = true;
            }
            else
            {
                if (GetExcludedLoggers().Any(i => logger.StartsWith(i)))
                {
                    logThisEvent = false;
                }
                else
                {
                    logThisEvent = true;
                }
            }

            return logThisEvent;
        }

        /// <summary>
        /// Get's the comma separated string from config and splits them to a list of string
        /// </summary>
        /// <returns></returns>
        private List<string> GetExcludedLoggers()
        {
            List<string> lstExcludedLoggers = new List<string>();

            try
            {
                if (!string.IsNullOrWhiteSpace(ExcludeLoggers))
                {
                    lstExcludedLoggers.AddRange(ExcludeLoggers.Split(',').ToList());
                }
            }
            catch (Exception ex)
            {

            }

            return lstExcludedLoggers;
        }

        /// <summary>
        /// get the Common Log Event Format (CLEF) json string reqd. for Seq, from the logging event
        /// </summary>
        /// <param name="loggingEvent"></param>
        /// <returns></returns>
        private string GetSeqClefJson(LoggingEvent loggingEvent)
        {
            string content = string.Empty;
            Exception exp = GetException(loggingEvent);
            string expString = exp != null ? Escape(exp.ToString()) : string.Empty;
            string message = GetLogMessage(loggingEvent, exp);
            message = Escape(message);
            StringBuilder sb = new StringBuilder();

            sb.Append("{");

            /// The Seq specific properties for Seq
            /// https://docs.datalust.co/docs/posting-raw-events#reified-properties
            sb.Append(GetPayloadJson("@t", loggingEvent.TimeStamp.ToString("dd-MMM-yyyy HH:mm:ss"), false));
            sb.Append(GetPayloadJson("@m", message, true));
            sb.Append(GetPayloadJson("@l", loggingEvent.Level.Name, true));
            sb.Append(GetPayloadJson("@x", expString, true));

            /// You can include additional properties here
            sb.Append(GetPayloadJson("Application", ApplicationName, true));
            sb.Append(GetPayloadJson("Logger", loggingEvent.LoggerName, true));

            sb.Append("}");
            content = sb.ToString().Replace("\r\n", " ");
            return content;
        }

        /// <summary>
        /// returns the exception object from the loggingevent
        /// </summary>
        /// <param name="loggingEvent"></param>
        /// <returns></returns>
        private Exception GetException(LoggingEvent loggingEvent)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(loggingEvent.GetExceptionStrRep()))
                {
                    var exception = GetInstanceField(typeof(LoggingEvent), loggingEvent, "m_thrownException");
                    return exception as Exception;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                        | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }

        /// <summary>
        /// Formats the string by adding escape sequence for special characters
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string Escape(string s)
        {
            if (s == null)
            {
                return (string)null;
            }

            StringBuilder stringBuilder = (StringBuilder)null;
            int startIndex = 0;
            char ch;

            for (int index = 0; index < s.Length; ++index)
            {
                ch = s[index];

                if (ch < ' ' || ch == '\\' || ch == '"')
                {
                    if (stringBuilder == null)
                    {
                        stringBuilder = new StringBuilder();
                    }

                    stringBuilder.Append(s.Substring(startIndex, index - startIndex));
                    startIndex = index + 1;

                    switch (ch)
                    {
                        case '\t':
                            stringBuilder.Append("\\t");
                            continue;
                        case '\n':
                            stringBuilder.Append("\\n");
                            continue;
                        case '\f':
                            stringBuilder.Append("\\f");
                            continue;
                        case '\r':
                            stringBuilder.Append("\\r");
                            continue;
                        case '"':
                            stringBuilder.Append("\\\"");
                            continue;
                        case '\\':
                            stringBuilder.Append("\\\\");
                            continue;
                        default:
                            stringBuilder.Append("\\u");
                            stringBuilder.Append(((int)ch).ToString("X4"));
                            continue;
                    }
                }
            }

            if (stringBuilder == null)
            {
                return s;
            }

            if (startIndex != s.Length)
            {
                stringBuilder.Append(s.Substring(startIndex));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// assigns the value to the 'message' property of Seq
        /// First, check for the RenderedMessage
        /// If it is empty, use the MessageObject
        /// If it is an exception, then add the exception's message to the existing message.
        /// </summary>
        /// <param name="loggingEvent"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private string GetLogMessage(LoggingEvent loggingEvent, Exception exception)
        {
            string logMessage = loggingEvent.RenderedMessage;
            logMessage = string.IsNullOrWhiteSpace(logMessage) ? Convert.ToString(loggingEvent.MessageObject) : logMessage;

            if (exception != null)
            {
                if (!string.IsNullOrWhiteSpace(logMessage))
                {
                    logMessage = logMessage + "\r\n" + exception.Message;
                }
                else
                {
                    logMessage = exception.Message;
                }
            }

            return logMessage;
        }

        /// <summary>
        /// returns the json string of the payload i.e the key & its value
        /// </summary>
        /// <returns></returns>
        private string GetPayloadJson(string key, string value, bool prependDelimiter)
        {
            string payload = string.Empty;
            payload = "\"" + key + "\":" + "\"" + value + "\"";

            if (prependDelimiter)
            {
                payload = "," + payload;
            }

            return payload;
        }
    }
}
