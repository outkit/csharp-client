using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Outkit
{
    public class Client
    {
        public string Key { get; set; }
        public string Secret { get; set; }
        public string Passphrase { get; set; }
        public string BaseUri { get; set; } = "https://api.outkit.io/v1";
        private readonly HttpClient _client;
        private readonly string _boundary = $"----------{Guid.NewGuid():N}";

        public Client(string key, string passPhrase, string secret = "", string uri = "")
        {
            Key = key;
            Passphrase = passPhrase;
            Secret = secret;
            BaseUri = string.IsNullOrEmpty(uri) ? BaseUri : uri;

            _client = new HttpClient();

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("User-Agent", "outkit-c#-client");
        }

        public OutkitResponse GetMessage(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var uri = BaseUri + "/messages/" + id;
            var sigData = GetSignatureData("GET", uri);
            var doc = Get(uri, sigData);
            var elem = doc.RootElement.GetProperty("data");
            return new OutkitResponse(elem);
        }

        public List<OutkitResponse> GetAllMessages()
        {
            var uri = BaseUri + "/messages";
            var sigData = GetSignatureData("GET", uri);
            var doc = Get(uri, sigData);
            var arr = doc.RootElement.GetProperty("data");

            return arr.EnumerateArray().Select(elem => new OutkitResponse(elem)).ToList();
        }

        public OutkitResponse CreateMessage(Message message)
        {
            var json = message.ToJson();
            json = "{\"message\":" + json + "}";
            var uri = BaseUri + "/messages";
            var sigData = GetSignatureData("POST", uri, json);
            JsonDocument elem;
            if (message.HasAttachments)
            {
                var form = message.ToFormData(_boundary);
                 elem = Post(uri, form, sigData);
            }
            else
            {
                elem = Post(uri, json, sigData);
            }
            return new OutkitResponse(elem.RootElement);
        }
        
        private Dictionary<string,string> GetSignatureData(string method, string uri, string body = "")
        {
            var uriParts = new Uri(uri);
            var path = uriParts.AbsolutePath;
            var qs = uriParts.Query;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (!string.IsNullOrEmpty(qs)) 
            {
                body += "?" + qs;
            }
            var payload = timestamp + method + path + body;
            var pb = Encoding.UTF8.GetBytes(payload);
            var hmac = new HMACSHA256(pb);
            var hashBytes = hmac.ComputeHash(pb);
            var signature = Convert.ToBase64String(hashBytes);
            var ret = new Dictionary<string, string>
            {
                {"key", Key},
                {"signature", signature},
                {"timestamp", timestamp.ToString()},
                {"passphrase", Passphrase}
            };
            return ret;
        }

        private JsonDocument Get(string url, IReadOnlyDictionary<string, string> sig)
        {
            var msg = new HttpRequestMessage(HttpMethod.Get,url);

            AddHeaders(msg, sig);

            var response = _client.SendAsync(msg).Result;
            if (response.IsSuccessStatusCode)
            {
                var st = response.Content.ReadAsStringAsync().Result;
                return JsonDocument.Parse(st);
            }

            var error = response.Content.ReadAsStringAsync().Result;
            throw new Exception($"{response.StatusCode},{error}");
        }

        private JsonDocument Post(string url, string body, IReadOnlyDictionary<string, string> sig)
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, url);

            AddHeaders(msg, sig);
            msg.Content = new StringContent(body, Encoding.UTF8, "Application/Json");

            var response = _client.SendAsync(msg).Result;
            if (response.IsSuccessStatusCode)
            {
                var st = response.Content.ReadAsStringAsync().Result;
                return JsonDocument.Parse(st);
            }

            var error = response.Content.ReadAsStringAsync().Result;
            throw new Exception($"{response.StatusCode},{error}");

        }

        private JsonDocument Post(string url, HttpContent body, IReadOnlyDictionary<string, string> sig)
        {
            body.Headers.Add("Outkit-Access-Key", sig["key"]);
            body.Headers.Add("Outkit-Access-Signature", sig["signature"]);
            body.Headers.Add("Outkit-Access-Timestamp", sig["timestamp"]);
            body.Headers.Add("Outkit-Access-Passphrase", sig["passphrase"]);

            var response = _client.PostAsync(url, body).Result; 
            if (response.IsSuccessStatusCode)
            {
                var st = response.Content.ReadAsStringAsync().Result;
                return JsonDocument.Parse(st);
            }

            var error = response.Content.ReadAsStringAsync().Result;
            throw new Exception($"{response.StatusCode},{error}");
        }

        private static void AddHeaders(HttpRequestMessage msg, IReadOnlyDictionary<string, string> sig)
        {
            msg.Headers.Add("Outkit-Access-Key", sig["key"]);
            msg.Headers.Add("Outkit-Access-Signature", sig["signature"]);
            msg.Headers.Add("Outkit-Access-Timestamp", sig["timestamp"]);
            msg.Headers.Add("Outkit-Access-Passphrase", sig["passphrase"]);
        }
    }
}
