namespace UG_DT_InternalAmolCall.Models
{
    public class BaseResponse
    {
        public string ErrorCode { get; set; } = StatusCodes.failedCode;
        public string ErrorMessage { get; set; } = StatusMessages.baseResponse;
        public string Timestamp { get; set; } = DateTime.Now.ToString();
    }
}
