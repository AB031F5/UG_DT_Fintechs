namespace UG_DT_InternalAmolCall.Models
{
    public class Database
    {
        public const string PROD = "v1.0.1";
        public const string UAT = "server=ugpbhkmapp0002;port=3306;Database=amoldb;Uid=BillerUser;Pwd=B!ll3r@2022!;";
        public const string local = "server=127.0.0.1;port=3306;Database=amoldb;Uid=root;Pwd=Malaika27th";
        public const string Connection = local;
    }
    public class Swagger
    {
        public const string VERSION = "v1.0.1";
        public const string NAME = "Fintechs Api";
        public const string ENDPOINT = "/swagger/v1/swagger.json";
    }
    public class Jwt
    {
        public const string Key = "OJumkZ2ML3pq261wQIOsNUllSOZSO1ky";
        public const string Issuer = "https://localhost:7158//";
        public const string Audience = "https://localhost:7158//";
    }
    public static class StatusCodes
    {
        public const string successCode = "0";
        public const string PendingCode = "01";
        public const string failedCode = "100";
        public const string invalidRequest = "102";
        public const string noTranCode = "105";
        public static readonly int Status200OK;
        public static readonly string SuccessCode200 = "200";
        public static readonly string DuplicateCode = "200";
    }
    public static class ContextToShowTypes
    {
        public const string ALL = "ALL";
        public const string SUMMARY = "SUMMARY";
    }
    public class StatusMessages
    {
        public const string unauthorizedAccess = "Unauthorized access.";
        public const string accessDenied = "Access denied.";
        public const string baseResponse = "Unable to process request. Please try again later.";
        public const string success = "Success.";
        public const string tokenInvalid = "Invalid token.";

        public const string successFulMessage = "Transfer completed successfully";
        public const string genericErrorMessage = "Error Processing the request please try again later";
        public const string databaseError = "Database Error, Please contact systems admi9nistrator";

        public const string pendingMessage = "Transaction status is pending, check transaction status";
        public const string duplicateMessage = "Duplicate Transaction ID";
        public const string noTranMessage = "Transaction ID not found";
        public const string tranFailedMessage = "Transaction Failed";
        public const string invalidRequestMessage = "Invalid request";
        public const string exceptionMessageMessage = "An error has occured, please try again later";
        public const string InvalidCurrency = "Invalid Currency";
        public const string InvalidToken = "Invalid Token";

    }
    public class LogConstants
    {
        public const string local = @"C:\Users\Ab031f5\Documents\YoLogs\";
        public const string UAT_SERVER_LOG = @"C:\Logs\AMOL_Endpoints\";
        public const string PROD_SERVER_LOG = @"C:\Logs\AMOL_Endpoints\";
        public const string Errorlogs = UAT_SERVER_LOG;
    }
    public class StoredProcedure
    {
        public const string CHECK_DUPLICATE = "_ServiceTxnByReference";
        public const string SAVE_TXN = "_ServiceSaveTxn";
        public const string UPDATE_TXN = "_ServiceUpdateTxn";
    }
}
