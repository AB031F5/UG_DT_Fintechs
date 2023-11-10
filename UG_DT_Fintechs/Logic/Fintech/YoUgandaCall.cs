using Newtonsoft.Json;
using UG_DT_InternalAmolCall.Models;
using UG_DT_InternalAmolCall.Models.Fintech.YoUganda;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Security.Cryptography.Xml;
using System.Collections;
using Org.BouncyCastle.Asn1.Ocsp;
using UG_DT_Fintechs.Logic;

namespace UG_DT_InternalAmolCall.Logic.Fintech
{
    public class YoUgandaCall
    {
        public async Task<HttpResponseMessage> SendPaymentNotification(YoPaymentNoti requestBody)
        {
            ArrayList log = new ArrayList();
            Helpers helpers = new Helpers();
            try
            {
                var obj = new
                {
                    datetime = $"{requestBody.datetime}",
                    method = $"{requestBody.method}",
                    dataFields = new
                    {
                        dateTime = $"{requestBody.dataFields.dateTime}",
                        accountNumber = $"{requestBody.dataFields.accountNumber}",
                        accountName = $"{requestBody.dataFields.accountName}",
                        type = $"{requestBody.dataFields.type}",
                        amount = $"{requestBody.dataFields.amount}",
                        amountInForeignCurrency = $"{requestBody.dataFields.amountInForeignCurrency}",
                        foreignCurrency = $"{requestBody.dataFields.foreignCurrency}",
                        exchangeRate = $"{requestBody.dataFields.exchangeRate}",
                        narrative = $"{requestBody.dataFields.narrative}",
                        reference = $"{requestBody.dataFields.reference}"
                    }
                };
                string rqContent = JsonConvert.SerializeObject(obj);
                log.Add($"++++++++++ Starting Http Post Request to {requestBody.endpoint} ::::");
                log.Add($"++++++++++ {requestBody.txnType} Request Bdy {rqContent} ::::");
                string signature = ComputeSignature(rqContent, requestBody);
                string hmac = ComputeHmac(rqContent, requestBody.hmacKey, requestBody);

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, requestBody.endpoint);
                request.Headers.Add("x-hmac", $"{hmac}");
                request.Headers.Add("x-signature", $"{signature}");
                var content = new StringContent(rqContent, null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);

                helpers.writeToFile(log, requestBody.service);
                return response;
            }
            catch (Exception ex)
            {
                log.Add($"++++++----@@@@@@@@@@@@@@@@@ Exception :::: {ex.Message} ");
                log.Add($"++++++----@@@@@@@@@@@@@@@@@ Stack :::: {ex.StackTrace} ");
                helpers.writeToFile(log, requestBody.service);
                throw new Exception(ex.StackTrace);
            }
        }

        private string ComputeSignature(string data, YoPaymentNoti requestBody)
        {
            ArrayList log = new ArrayList();
            Helpers helpers = new Helpers();
            try
            {
                log.Add($"++++++---- Generating Signature ");
                using (var rsa = RSA.Create())
                {
                    var privateFile = $@"-----BEGIN PRIVATE KEY-----
MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCcwGAYHwg7DEiG
Nsj3D9j8yEBxRm8B02ewAIMldd+s9E57in78SpPLPBj8G2SbsFbXIB7QsFs6tAQz
baRjp0O6qAfGkx78Ue36Wg0eyWwp+KkkAUBHFUxFXSm80js5TZ0x/0I0xoovP9Cj
2+a2nwfowzpI0MxEBJw1yJpohxE5PNlHNt/VYrdkZeXaaMy2+WE60RQFEI4tauOJ
BUMPXvYDnekM17wcmDlEfceoEu33zJVjSQ90mL/9+3Iix63f7+eYnxFaTPCiNloj
eqk5PbYLL8eiEn4dQzyQOekzMiiLQOkkm8PAPtNapsoolxEjpT8yOKevd18ZbW03
N9Ls0KBFAgMBAAECggEACmZ9Obx9XaWaqO+49SOOYHSiSfcZZcPywkEWcoWCpMVv
m6TRzh2IoYMdnpnsBU8gQ+p0APa35t+LfnoM3ovPjkft1CVuIzcFv4b6uIs2wQYo
UCTrxeD04Y6lNqEK9ZS1FFx9C7wMmRv2f5jB6GYJg/kdbzgOBYlPcAYdBfMdl8WG
KDgRb5Dpv7I9+OsS07mWLcmOJVRsKVxdEvHVk8goyIKdAYLyG+a708VgoHMnXDLx
p5UOqX7lrpT21V2/GkAeaP6eB9JphqW+EX1jjrvMdae8Q/hELNO67w6x0RwD9qd6
8RCqzRn968cBiTxeQXZ4LGu376mfdqqsNAh2A3VSAQKBgQC86LfvOfoRAiBX8c6b
0/5wxN1pArx+rrNKdyhgt1E50Bk+QRnCTs37HLsttFmfhedVAwbjaLdKA+Yd5SnM
3Wf+BKjVKfZPee+nNbcaWLeW9cLjGC28+VR1PPUenHJqGuxHs1KAjMDxANMfsFym
S3Gc4W/kILMdpHVJy6Ph6FeSxQKBgQDUa/HmcYyQSkqO9Xkjjc5CoCjuuE+ducFx
gi20s1ycjAU8PpEyGHhTtmDKKOEsuqURmYoUvUWS7xNDgFM4R5h+0I9w7tNmhCRG
X2bAa+ARdRAnECHbMi+Djh/dnc3UdQiwPxcmU+JXDQ+AdZCeMR3GNwscznlDdg9r
p78St5+vgQKBgH3ehef09mTsyMYwTNzRZOCYm3pDo3q5RqcgPBwtKQbfXGJ5mAM6
5M4jd6hdWbYLz3Z6XNWbST0c2fAjaDWjdI3xZtkZa4/LDF6aUNVSNYl1WRRdYORg
MOEo654o2adPJw4jMp6Kqmgqh4G+zgzTifDg2N/k1dOzZ57y+9AjMm6NAoGACU8R
sr+XIVugGNO5E03LlC/gm1WIZ6kUSR0jU5/olxOrxxAW3NMlssVzSGiyNXuYcEUC
QBZrTh0cAwFEpFq+3A/XuWM98GgqDstvfU886obuxkd2tFxmqKU50ERyGLVoRBD/
urZCXP49h6ufQs90NpOC8Sg7ODGcS2N2hCN35AECgYB5KVsGdrYrsFwKwehBZbyO
QAXZuTo3NGolQuTvd/r9eTw8iaFH/KAOQqfRc/K6TFMXIdEHy/k1nJWuNVcJLufd
GJMZt/6KQisdvhOWJugOHk6fiv3UE89UDfd1M+Rx5plSu25tAeCcT8VBj4NlGo46
SaThEVjCkvIBARR3NBvX2w==
-----END PRIVATE KEY-----
";
                    log.Add($"++++++---- Using PK Private {privateFile} ");
                    rsa.ImportFromPem(privateFile.ToCharArray());

                    var dataToSign = Encoding.UTF8.GetBytes(data);
                    log.Add($"++++++---- Byte generated {dataToSign} ");
                    var signature = rsa.SignData(dataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    log.Add($"++++++---- Signature generated {Convert.ToBase64String(signature)} ");
                    helpers.writeToFile(log, requestBody.service);
                    return Convert.ToBase64String(signature);
                }
            }
            catch (Exception ex)
            {
                log.Add($"++++++----@@@@@@@@@@@@@@@@@ Exception :::: {ex.Message} ");
                log.Add($"++++++----@@@@@@@@@@@@@@@@@ Stack :::: {ex.StackTrace} ");
                helpers.writeToFile(log, requestBody.service);
                throw new Exception(ex.Message);
            }
        }
        private string ComputeHmac(string data, string key, YoPaymentNoti requestBody)
        {
            ArrayList log = new ArrayList();
            Helpers helpers = new Helpers();
            key = key ?? "";
            log.Add($"++++++--- Computing Hmac on {data} using key {key} ");
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                string s = Convert.ToBase64String(hash);
                log.Add($"+++++++--- Hmac ready :: {sb.ToString()} ::::");
                helpers.writeToFile(log, requestBody.service);
                return sb.ToString();
            }

        }
        private bool VerifySignature(string data, string signature)
        {
            using (var rsa = RSA.Create())
            {
                var publicFile = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnMBgGB8IOwxIhjbI9w/Y
/MhAcUZvAdNnsACDJXXfrPROe4p+/EqTyzwY/Btkm7BW1yAe0LBbOrQEM22kY6dD
uqgHxpMe/FHt+loNHslsKfipJAFARxVMRV0pvNI7OU2dMf9CNMaKLz/Qo9vmtp8H
6MM6SNDMRAScNciaaIcROTzZRzbf1WK3ZGXl2mjMtvlhOtEUBRCOLWrjiQVDD172
A53pDNe8HJg5RH3HqBLt98yVY0kPdJi//ftyIset3+/nmJ8RWkzwojZaI3qpOT22
Cy/HohJ+HUM8kDnpMzIoi0DpJJvDwD7TWqbKKJcRI6U/Mjinr3dfGW1tNzfS7NCg
RQIDAQAB
-----END PUBLIC KEY-----
";
                rsa.ImportFromPem(publicFile.ToCharArray());

                var dataToVerify = Encoding.UTF8.GetBytes(data);
                var signatureData = Convert.FromBase64String(signature);
                return rsa.VerifyData(dataToVerify, signatureData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }
    }
}
