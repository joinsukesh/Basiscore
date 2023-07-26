namespace Basiscore.CmsAudit.Models
{
    public class BaseResponse
    {
        public BaseResponse()
        {
            StatusCode = 0;
            StatusMessage = string.Empty;
            ErrorMessage = string.Empty;
        }

        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string ErrorMessage { get; set; }
    }
}