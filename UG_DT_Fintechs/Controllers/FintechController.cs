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
        [Authorize]
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
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"].ToString();
                    var credentialSecretKey = AuthenticationHeaderValue.Parse(authHeader).Parameter;

                    log.Add($"Auth :::: {authHeader}");

                    log.Add($"SecretKey :::: {credentialSecretKey}");

                    var userClaims = identity.Claims;//process request

                    if (userClaims != null)
                    {
                        log.Add($"All Claims :::: {userClaims}");
                        string? name = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value.ToString();
                        log.Add($"Name Claim :::: {name}");
                        if (name.Equals(request.service))
                        {
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
                        else
                        {
                            log.Add($"Request Bdy Service name ::{request.service}:: does not equal to Auth Bdy name ::{name}");
                            return Unauthorized(new UnauthorizedResponse());
                        }
                    }
                }
                helpers.writeToFile(log, request.service);
                return Unauthorized(new UnauthorizedResponse());
            }
            catch (Exception ex)
            {
                log.Add($"@@@@@@@@ Exception : PushTxnHistoryNotification :::: {ex.Message}");
                log.Add($"@@@@@@@@ Stack : {ex.StackTrace}");
                helpers.writeToFile(log, request.service);
                return BadRequest(new { error = ex.Message });
            }

        }
    }
}