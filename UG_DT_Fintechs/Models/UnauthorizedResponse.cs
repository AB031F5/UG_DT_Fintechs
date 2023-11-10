namespace UG_DT_InternalAmolCall.Models
{
    public class UnauthorizedResponse
    {
        public string ErrorCode { get; set; } = StatusCodes.failedCode;
        public string ErrorMessage { get; set; } = StatusMessages.unauthorizedAccess;
        public string Timestamp { get; set; } = DateTime.Now.ToString();
    }
}
