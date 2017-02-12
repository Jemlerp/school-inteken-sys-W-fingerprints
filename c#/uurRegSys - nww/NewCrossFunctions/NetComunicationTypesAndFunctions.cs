﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NewCrossFunctions {
    public class NetComunicationTypesAndFunctions {

        private static ServerResponse WebRequest(ServerRequest _request, string _apiAddres) {
            using (HttpClient client = new HttpClient()) {
                HttpResponseMessage response = client.PostAsync(_apiAddres, new StringContent(JsonConvert.SerializeObject(_request), Encoding.UTF8, "application/json")).Result;
                Task<string> result = response.Content.ReadAsStringAsync();
                //string banana = JsonConvert.DeserializeObject<string>(result.Result);
                return JsonConvert.DeserializeObject<ServerResponse>(result.Result);
            }
        }

        /* bleh
        private static ServerResponse WebRequest(ServerRequest _Request, string _APIAddres) {
            using (HttpClient httpClient = new HttpClient()) {
                httpClient.DefaultRequestHeaders.Add("X-Accept", "application/Json");                
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync(_APIAddres, _Request);
                response.Wait();
                Task<string> result = response.Result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ServerResponse>(result.Result);
            }
        }
        */

        public static ServerResponse WebRequest(object request, string _Username, string _Password, string _ApiAddres) {
            ServerRequest reques = new ServerRequest();
            reques.UserName=_Username;
            reques.Password=_Password;
            reques.Request=request;
            return WebRequest(reques, _ApiAddres);
        }

        public class ServerRequest {
            public string UserName { get; set; }
            public string Password { get; set; }
            public object Request { get; set; }
        }

        public class ServerResponse {
            public bool IsErrorOccurred { get; set; } = false;
            public ERRORINFO ErrorInfo { get; set; } = new ERRORINFO();
            public object Response { get; set; }
        }

        public class ERRORINFO {
            public string ErrorMessage { get; set; }
        }

        public interface IKnow {
            WhatIsThisEnum WatIsDit { get; }                 
        }
        
        public enum WhatIsThisEnum {
            RSqlServerDateTime,
            RInteken,
            ROneDateRegiOverzight,
            RChangeRegTable,               
        }

        public class ServerRequestSqlDateTime : IKnow {
            public WhatIsThisEnum WatIsDit { get { return WhatIsThisEnum.RSqlServerDateTime; } }
        }

        public class serverResponseSqlDateTime {
            public DateTime SqlDateTime { get; set; }
        }


        public class ServerRequestTekenInOfUit : IKnow{
            public WhatIsThisEnum WatIsDit { get { return WhatIsThisEnum.RInteken; } }
            public string NFCCode { get; set; }
        }

        public class ServerResponseInteken {
            public DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry TheUserWithEntryInfo { get; set; } = new DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry();
            public bool ingetekened { get; set; }
            public bool uitgetekened { get; set; }
            public bool uitekenengeanuleerd { get; set; }
        }

        public class ServerRequestOverzightFromOneDate : IKnow {
            public WhatIsThisEnum WatIsDit { get { return WhatIsThisEnum.ROneDateRegiOverzight; } }
            public bool useToday { get; set; } = false;
            public bool alsoReturnExUsers { get; set; } = false;
            public DateTime dateToGetOverzightFrom { get; set; }
        }

        public class ServerResponseOverzightFromOneDate {
           public List<DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry> EtList { get; set; } = new List<DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry>();
           public DateTime SQlDateTime { get;set; }
        }

        public class ServerRequestChangeRegistratieTable : IKnow {
            public WhatIsThisEnum WatIsDit { get { return WhatIsThisEnum.RChangeRegTable; } }
            public bool isNieuwEntry { get; set; } = false; //als true ignore DatabaseTypesAndFunctions.RegistratieTableTableEntry.ID
            public bool newEntryDateIsToday { get; set; } = true;
            public DatabaseTypesAndFunctions.RegistratieTableTableEntry deEntry { get; set; } = new DatabaseTypesAndFunctions.RegistratieTableTableEntry();
        }

        public class ServerResponseChangeRegistratieTable {
            //public DatabaseTypesAndFunctions.RegistratieTableTableEntry deEntry { get; set; } // voor DatabaseTypesAndFunctions.RegistratieTableTableEntry.ID als het nietuw as //SCOPE_IDENTITY() wwerrk nie
        }

    }
}
