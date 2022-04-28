using Api.ApiGateway.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Api.ApiGateway.Handlers
{
    public class HeaderDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HeaderDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IEnumerable<string> headerValues;
            string decrypt = string.Empty;
            string urlAbsolute = string.Empty;
            if (request.Headers.TryGetValues("Authorization", out headerValues))
            {
                //only in case necessary transform data headers
                //string accessToken = headerValues.First();
                //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                //request.Headers.Remove("Bearer");
            }

            if ((request.Method.Method == "POST" || request.Method.Method == "PUT") && (!"multipart/form-data".Equals(request.Content.Headers.ContentType.MediaType)))
            {
                string cifra = "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";
                //string cifra = "vPYECYSTbmzMpv6ZFkaADD3NfR2qWeXheRDxMztlpTon5oOF/MNIb/u4DAsGvAM4dt5LIFGdnM3vVXpC/ReX7H6rrTUitpE/gVJYoGj7jpRDsCZS4hc0N4HK6otu6AK9bNaJM1MTlUg3wKJVRTc0Q1ZSKDhAzAQYzrt4xpNboiulXhBo6D/XhSI5grQjWHK+GsrVcyq7ENxNdgCaGKkcnScXQqAkPO2Vt+DR06ntucQDCKGniKEwpi/JZ7EqVcSkaKTWjyc68bcGY/rezOdflY3CdS1Kvdxh11wzD8XczXqTT8EmVxmcjaen+peXPsnX";
                var u = Crypto.DecryptString(cifra);
                string getDataEncrypt = GetDataEncrypt(request);                
                var decriptedFromJavascript = Crypto.DecryptString(getDataEncrypt);

                if (!string.IsNullOrEmpty(decriptedFromJavascript))
                {
                    StringContent data = new StringContent(decriptedFromJavascript, Encoding.UTF8, "application/json");
                    request.Content = data;
                }
            }
            #region Tratamiento Peticiones GET
            //else if (request.Method.Method == "GET")
            //{
            //    var parameters = new System.Text.StringBuilder();
            //    string[] url = request.RequestUri.LocalPath.Split('/');
            //    for (int pos = 4; pos < url.Length; pos++)
            //    {
            //        parameters.Append(url[pos]);
            //        if (pos + 1 != url.Length)
            //        {
            //            parameters.Append("/");
            //        }
            //    }
            //    decrypt = Crypto.DecryptString(parameters.ToString());
            //    var urlPath = CreateUrl(request.RequestUri, decrypt);
            //    request.RequestUri = new Uri(urlPath);
            //}
            #endregion Tratamiento Peticiones GET

            var response = await base.SendAsync(request, cancellationToken);
            response.Content = new StringContent(Crypto.EncryptString(response.Content.ReadAsStringAsync().Result.ToString()));
            var textPlain = Crypto.DecryptString(response.Content.ReadAsStringAsync().Result.ToString());

            return response;
        }

        public static string GetDataEncrypt(HttpRequestMessage body)
        {
            ApplicationResponseGeneric<string> appResponse = null;
            appResponse = JsonConvert.DeserializeObject<ApplicationResponseGeneric<string>>(body.Content.ReadAsStringAsync().Result);
            return appResponse.Data;
        }                

        private string CreateUrl(Uri requestUri, string parameters)
        {
            var urlpath = new System.Text.StringBuilder();
            string[] url = requestUri.OriginalString.Split('/');
            for (int pos = 0; pos < 6; pos++)
            {
                urlpath.Append(url[pos]+"/");
            }
            urlpath.Append(string.Join("", parameters.Split('\"')));
            return (urlpath).ToString();
        }
        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
        {
            HttpRequestMessage clone = new HttpRequestMessage(req.Method, req.RequestUri);

            // Copy the request's content (via a MemoryStream) into the cloned object
             var ms = new MemoryStream();
            if (req.Content != null)
            {
                await req.Content.CopyToAsync(ms).ConfigureAwait(false);
                ms.Position = 0;
                clone.Content = new StreamContent(ms);

                // Copy the content headersif (req.Content.Headers != null)
                foreach (var h in req.Content.Headers)
                {
                    clone.Content.Headers.Add(h.Key, h.Value);
                }                    
            }

            clone.Version = req.Version;

            foreach (KeyValuePair<string, object> prop in req.Properties)
                clone.Properties.Add(prop);

            foreach (KeyValuePair<string, IEnumerable<string>> header in req.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }                
    }

    public class ApplicationResponseGeneric<T>
    {
        public T Data { get; set; }
    }


}
