using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;
using System.Net.Http.Headers;
using System.Security.Claims;
using UG_DT_Fintechs.Logic;
using UG_DT_InternalAmolCall.Logic.Fintech;
using UG_DT_InternalAmolCall.Models.Fintech.YoUganda;
using UG_DT_InternalAmolCall.Models;

namespace UG_DT_Fintechs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FintechController : ControllerBase
    {
        [HttpPost]
        [Route("PushYoUgandaPayment")]
        public async Task<IActionResult> PushTxnHistoryNotification(YoPaymentNoti request)
        {
            BaseResponse baseResponse = new BaseResponse();
            var currentUser = HttpContext.User;

            ArrayList log = new ArrayList();
            ArrayList log2 = new ArrayList();
            Helpers helpers = new Helpers();
            try
            {
                log.Add("------------------------------------------------- PushTxnHistory -------------------------------------------------");
                log.Add($"Name Claims :::: is Valid");
                YoUgandaCall yo = new YoUgandaCall();
                log.Add($"API Request Bdy :::: {JsonConvert.SerializeObject(request)}");
                helpers.writeToFile(log, request.service);
                HttpResponseMessage yoResponseMessage = await yo.SendPaymentNotification(request);
                log2.Add($"HttpResponseMessage :::: {yoResponseMessage}");
                String yoJsonResponse = await yoResponseMessage.Content.ReadAsStringAsync();
                log2.Add($"yoJsonResponse :::: {yoJsonResponse}");
                if (yoResponseMessage != null && yoResponseMessage.IsSuccessStatusCode)
                {
                    YoResponse yoResponse = JsonConvert.DeserializeObject<YoResponse>(yoJsonResponse.ToString());
                    log2.Add($"yoJsonResponse Serialized :::: {JsonConvert.SerializeObject(yoResponse)}");
                    helpers.writeToFile(log2, request.service);
                    return Ok(yoResponse);
                }
                else
                {
                    log2.Add($"BadRequest or Invalid Response");
                    helpers.writeToFile(log2, request.service);
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                log.Add($"@@@@@@@@ Exception : PushTxnHistoryNotification :::: {ex.Message}");
                log.Add($"@@@@@@@@ Stack : {ex.StackTrace}");
                helpers.writeToFile(log, request.service);
                return BadRequest(new { error = ex.Message });
            }

        }

        [HttpPost]
        [Route("VerifySignature")]
        public IActionResult VerifyData(YoPaymentNoti request)
        {
            try
            {
                YoUgandaCall yo = new YoUgandaCall();
                var isVerified = yo.Verify(request);
                return Ok(new {isDataVerified = isVerified });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
                throw;
            }
        }
    }
}