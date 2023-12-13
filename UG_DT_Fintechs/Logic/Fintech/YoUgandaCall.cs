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
                request.Headers.Add("X-IBM-Client-Secret", $"rY8bG5kG2hB4qK5nE8vK1vI1dS3hO7sL8iU5bT6rD8eD5iJ4lA");
                request.Headers.Add("X-IBM-Client-Id", $"0bc2e507-787e-4f1d-a33e-536819a54653");
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
                return null;
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
MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCTor9inK+5j0jd
HOkpR/4EHuW5SSDEMGJLdHfcUApBWnkjUUNlfzcIkCoDKH+QOoiV1ir8aiemWVlp
YqPn68D+xWWNv5SE9CLS8Z/Oxb+3u6xqQuPqLk2pwrP9rlO9eIrfn7Z4ZoghoL6I
y4GRz31hCDpMtLtTLRZDad68VZ8y4E+/i6BxYunVOFMG/LzZsqBhc3XmvTNrucEw
amLsH057oINPUzU/GmEjRHYxfK6sMxUjI/YrGPO39yau/0ppnqnAswaXN8WOx9uE
FtiIEeNAqhxi+oej9CLOXvJBDstNcZpoB6uZiR/ohZTx+FUgXJJsm4PBalDTJzo2
kGIEUCybAgMBAAECggEAGQ+2d1Hrzo8RKl+/AL/00dRA4NwWENe5x99EfYSkyQVH
rQQdcbxPpkd6qkCUrvIQv4a0k1ad1nIyaF5TPopZn6X4oEBWQN+EjdRwcVRGQDU+
hmdGczla+6dOkJCoLHCq+l0NR5D4KOU1ktkg7JSRTwhrhRvk9LChp93v7n6GYMdL
DJjtSzX7BuGOKqezImj7PDdmE7mHiM00ZZZg0R+4/sAgLrd7HALQhDPF1dx/3TPT
K0TP0BC06zpHX7p41+A3tb6YsXFH+Ix/GXQT6FcpmZY3HHCzXY1IjipVqmyp4WSc
SEOPoGlltpkGCTepRpeIx2oKOSU8A1rAwnS2tK76mQKBgQDMUQ5pG/7jijyRsY6t
CYjb0AGKtUUezHpjBBkHpZgeHcTHK3RcYXyK2+L3WvkINJLbn4QC1ZpKCb0FFWxc
sqTWuMrNju/izkXRmOZOMnh7irmzSksQTHv5VJKZ82u710KCsKlg6IXglrHSMRrG
9zIMvO5xYM/9KsvlapdPvb9xZQKBgQC4+zQQ4Oyuzq+ztMqkDIq+z4+M6dI1qJiZ
0rG34lkGkT1ZofTxIZfvG4msFiHzbHj7YbTpcSRvpdLMBVyzcTo4JKQfWdVNpKFv
JXMyE5/ZgczF6J6IzSgsKGMSies3dDGV2AB6O7lPMo+cIo4b80FRXCBdzc268QgY
0x5YivZF/wKBgE4tJYpqP9joZ8yHV8Q1xbv1luFwAoaKTrncM4eNgOrlEQn0Qo2m
b3TiNbHXiTnug9Ks8mHzQRnbKAvt6ox9fjz6b/6/FmJ98pEEI/r/rMH4jp1fa+FM
opMAHSfyz+ILt1MyLVz7G16XympWh5xhsTDWiZWwKWUBHwslAchtHfdRAoGAYjxl
piENELiK7rCwfPZ8KGEqJvd1vzFN+UK/RpxaEvbG0fcEkZn5ie3h6xXF6fRNO5Lj
9KU1aGVz0Q138YPMG0y6LDwU3yry4IfO6WUqmuQnz2J97sCk+O5pzsqXs9dtDdqs
Rj25Z6m+QnjHZ2iD46U9OxO0kfa3Chf0IamKqBkCgYAzacutczRnkKEKfZOeqwbd
qX0wWeLeo76+uIZyobcfyJ/k0zmLP7gPYfaU1Lz2yxVYM9Ina8e2VyTj3ZyOH9lI
ipPZOg9/QL9ks8wtR0Lo5e+OjyVX3LRySbhWksqrxvp89L8tP/AChN/SeWsW5wLr
4BXM1oa2FapCZPGC5qqmcw==
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

        public bool Verify(YoPaymentNoti requestBody)
        {
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
                string signature = ComputeSignature(rqContent, requestBody);
                string hmac = ComputeHmac(rqContent, requestBody.hmacKey, requestBody);
                var verifyData = VerifySignature(rqContent, signature);
                return verifyData;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        private bool VerifySignature(string data, string signature)
        {
            using (var rsa = RSA.Create())
            {
                var publicFile = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAk6K/YpyvuY9I3RzpKUf+
BB7luUkgxDBiS3R33FAKQVp5I1FDZX83CJAqAyh/kDqIldYq/GonpllZaWKj5+vA
/sVljb+UhPQi0vGfzsW/t7usakLj6i5NqcKz/a5TvXiK35+2eGaIIaC+iMuBkc99
YQg6TLS7Uy0WQ2nevFWfMuBPv4ugcWLp1ThTBvy82bKgYXN15r0za7nBMGpi7B9O
e6CDT1M1PxphI0R2MXyurDMVIyP2Kxjzt/cmrv9KaZ6pwLMGlzfFjsfbhBbYiBHj
QKocYvqHo/Qizl7yQQ7LTXGaaAermYkf6IWU8fhVIFySbJuDwWpQ0yc6NpBiBFAs
mwIDAQAB
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
