using System.ComponentModel.DataAnnotations;

namespace UG_DT_InternalAmolCall.Models.Fintech.YoUganda
{
    public class Datafields
    {
        public string dateTime { get; set; } = DateTime.Now.ToString();
        public string accountNumber { get; set; } = "";
        public string accountName { get; set; } = "";
        public string type { get; set; } = "";
        public string amount { get; set; } = "";
        public string foreignCurrency { get; set; } = "";
        public string amountInForeignCurrency { get; set; } = "";
        public string exchangeRate { get; set; } = "";
        public string narrative { get; set; } = "";
        public string reference { get; set; } = "";
    }
    public class YoPaymentNoti
    {
        public string datetime { get; set; } = "";
        public string method { get; set; } = "transactionNotification";
        public string txnType { get; set; } = "YoPaymentNotification";
        public string service { get; set; } = "";
        public string hmacKey { get; set; } = "";
        public string endpoint { get; set; } = "";
        public Datafields dataFields { get; set; }
    }
}
