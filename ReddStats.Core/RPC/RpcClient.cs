using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using Newtonsoft.Json;

namespace ReddStats.Core.RPC
{
    using System.Configuration;

    public class RpcClient
    {
        protected static Uri Uri;

        protected static NetworkCredential Credentials;

        static RpcClient()
        {
            Credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["rpc.username"],
                ConfigurationManager.AppSettings["rpc.password"]);

            Uri = new Uri(ConfigurationManager.AppSettings["rpc.address"]);
        }

        public static T DoRequest<T>(string method, IList<Object> requestParams = null)
        {
            var client = new RpcRequest { method = method, requestParams = requestParams };

            string jsonRequest = JsonConvert.SerializeObject(client);

            string result = HttpCall(jsonRequest);

            var response = JsonConvert.DeserializeObject<RPCResponse<T>>(result);

            if (response.error != null)
            {
                throw new RPCException(response.error);
            }

            if (typeof(T).Name == typeof(string).Name)
            {
                return (T)(object)response.result.ToString();
            }

            return JsonConvert.DeserializeObject<T>(response.result.ToString());
        }

        protected static string HttpCall(string jsonRequest)
        {
            var request = (HttpWebRequest)WebRequest.Create(Uri);

            request.Method = "POST";
            request.ContentType = "application/json-rpc";

            // always send auth to avoid 401 response
            string auth = Credentials.UserName + ":" + Credentials.Password;
            auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth), Base64FormattingOptions.None);
            request.Headers.Add("Authorization", "Basic " + auth);

            //webRequest.Credentials = Credentials;

            request.ContentLength = jsonRequest.Length;

            using (var sw = new StreamWriter(request.GetRequestStream()))
            {
                sw.Write(jsonRequest);
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException wex)
            {
                using (var response = (HttpWebResponse)wex.Response)
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode != HttpStatusCode.InternalServerError)
                    {
                        throw;
                    }
                    return sr.ReadToEnd();
                }
            }
        }

        public class RPCResponse<T>
        {
            public T result;
            public RPCError error;
            public uint id;
        }

        public class RPCError
        {
            public int code;
            public string message;
        }

        class RPCException : Exception
        {
            public RPCError Error
            {
                get;
                private set;
            }

            public RPCException(RPCError rpcError)
                : base(rpcError.message)
            {
                this.Error = rpcError;
            }
        }
    }
}