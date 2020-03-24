// JsonRequest.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using Newtonsoft.Json;

namespace nUpdate.Administration.BusinessLogic.Http.JsonRpc
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonRequest
    {
        public JsonRequest()
        {
        }

        public JsonRequest(string method, object pars, object id)
        {
            Method = method;
            Params = pars;
            Id = id;
        }

        [JsonProperty("id")] public object Id { get; set; }

        [JsonProperty("jsonrpc")] public string JsonRpc => "2.0";

        [JsonProperty("method")] public string Method { get; set; }

        [JsonProperty("params")] public object Params { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}