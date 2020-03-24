// JsonRpcException.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace nUpdate.Administration.BusinessLogic.Http.JsonRpc
{
    /// <summary>
    ///     code        message             meaning
    ///     -32700      Parse error         Invalid JSON was received by the server.  An error occurred on the server while
    ///     parsing the JSON text.
    ///     -32600      Invalid Request     The JSON sent is not a valid Request object.
    ///     -32601      Method not found    The method does not exist / is not available.
    ///     -32602      Invalid params      Invalid method parameter(s).
    ///     -32603      Internal error      Internal JSON-RPC error.
    ///     -32000 to -32099 Server error   Reserved for implementation-defined server-errors.
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonRpcException : ApplicationException
    {
        public JsonRpcException(int code, string message, object data)
        {
            this.code = code;
            this.message = message;
            this.data = data;
        }

        [JsonProperty] public int code { get; set; }

        [JsonProperty] public object data { get; set; }

        [JsonProperty] public string message { get; set; }
    }
}