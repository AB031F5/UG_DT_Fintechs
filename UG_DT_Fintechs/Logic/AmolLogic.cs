using Newtonsoft.Json;
using System.Collections;
using UG_DT_Fintechs.Data;
using UG_DT_InternalAmolCall.Models;
using StatusCodes = UG_DT_InternalAmolCall.Models.StatusCodes;

namespace UG_DT_InternalAmolCall.Logic
{
    public class AmolLogic
    {
        DataBaseContext dbContext = new DataBaseContext();
        public bool UpdateTxn(string reference, string httpResponse, string httpResponseStatusCode, string amolResponse, string amolResponseStatusCode)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("P_txn_reference", reference);
            parameters.Add("P_http_response", httpResponse);
            parameters.Add("P_http_response_status_code", httpResponseStatusCode);
            parameters.Add("P_amol_response", amolResponse);
            parameters.Add("P_amol_response_status_code", amolResponseStatusCode);
            parameters.Add("P_status", "Completed");
            return dbContext.ExecuteNonQuery(StoredProcedure.UPDATE_TXN, parameters);
        }
    }
}
