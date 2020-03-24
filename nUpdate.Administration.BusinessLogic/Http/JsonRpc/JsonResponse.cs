// JsonResponse.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using Newtonsoft.Json;

namespace nUpdate.Administration.BusinessLogic.Http.JsonRpc
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "error")]
        public JsonRpcException Error { get; set; }

        [JsonProperty(PropertyName = "id")] public object Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "jsonrpc")]
        public string JsonRpc => "2.0";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "result")]
        public object Result { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class JsonResponse<T>
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "error")]
        public JsonRpcException Error { get; set; }

        [JsonProperty(PropertyName = "id")] public object Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "jsonrpc")]
        public string JsonRpc => "2.0";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "result")]
        public T Result { get; set; }
    }
}