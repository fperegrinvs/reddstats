namespace ReddStats.Core.RPC
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization = MemberSerialization.Fields)]
    public class RpcRequest
    {
        uint id = 1;
        public string method;

        [JsonProperty(PropertyName = "params", NullValueHandling = NullValueHandling.Ignore)]
        public IList<object> requestParams = null;
    }
}