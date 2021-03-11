using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Outkit
{
    public class Message
    {
        [JsonPropertyName("type")]
        public MessageType Type { get; set; }
        [JsonPropertyName("to")]
        public string To { get; set; }
        [JsonPropertyName("text_template_body")]
        public string TextTemplateBody { get; set; }
        [JsonPropertyName("text_body")]
        public string TextBody { get; set; }
        [JsonPropertyName("test")]
        public bool Test { get; set; }
        [JsonPropertyName("template_styles")]
        public string TemplateStyles { get; set; }
        [JsonPropertyName("template_id")]
        public string TemplateId { get; set; }
        [JsonPropertyName("template")]
        public string Template { get; set; }
        [JsonPropertyName("sync")]
        public bool Sync { get; set; }
        [JsonPropertyName("subject")]
        public string Subject { get; set; }
        [JsonPropertyName("render_only")]
        public bool RenderOnly { get; set; }
        [JsonPropertyName("project_id")]
        public string ProjectId { get; set; }
        [JsonPropertyName("project")]
        public string Project { get; set; }
        [JsonPropertyName("id_from_submitter")]
        public string IdFromSubmitter { get; set; }
        [JsonPropertyName("html_template_body")]
        public string HtmlTemplateBody { get; set; }
        [JsonPropertyName("html_body")]
        public string HtmlBody { get; set; }
        [JsonPropertyName("from")]
        public string From { get; set; }
        [JsonPropertyName("data")]
        public JsonElement Data { get; set; }
        [JsonPropertyName("backend_id")]
        public string BackendId { get; set; }
        [JsonPropertyName("backend")]
        public string Backend { get; set; }
        [JsonIgnore]
        public MessageDisposition Disposition { get; set; }

        public string ToJson()
        {
            // as there seems to be no way to remove null properties,  translate by hand.
            var res = "{";
            var t = Type == MessageType.email ? "email" : "sms";
            res += $"\"type\":\"{t}\",";
            if (!string.IsNullOrEmpty(To))
                res += $"\"to\":\"{To}\",";
            if (!string.IsNullOrEmpty(TextTemplateBody))
                res += $"\"text_template_body\":\"{TextTemplateBody}\",";
            if (!string.IsNullOrEmpty(TextBody))
                res += $"\"text_body\":\"{TextBody}\",";
            
            t = Test ? "true" : "false";
            res += $"\"test\":{t},";
            if (!string.IsNullOrEmpty(TemplateStyles))
                res += $"\"template_styles\":\"{TemplateStyles}\",";

            if (!string.IsNullOrEmpty(TemplateId))
                res += $"\"template_id\":\"{TemplateId}\",";

            if (!string.IsNullOrEmpty(Template))
                res += $"\"template\":\"{Template}\",";
            t = Sync ? "true" : "false";
            res += $"\"sync\":{t},";
            if (!string.IsNullOrEmpty(Subject))
                res += $"\"subject\":\"{Subject}\",";
            t = RenderOnly ? "true" : "false";
            res += $"\"render_only\":{t},";
            if (!string.IsNullOrEmpty(ProjectId))
                res += $"\"project_id\":\"{ProjectId}\",";

            if (!string.IsNullOrEmpty(Project))
                res += $"\"project\":\"{Project}\",";

            if (!string.IsNullOrEmpty(IdFromSubmitter))
                res += $"\"id_from_submitter\":\"{IdFromSubmitter}\",";
            if (!string.IsNullOrEmpty(HtmlTemplateBody))
                res += $"\"html_template_body\":\"{HtmlTemplateBody}\",";
            if (!string.IsNullOrEmpty(HtmlBody))
                res += $"\"html_body\":\"{HtmlBody}\",";
            if (!string.IsNullOrEmpty(From))
                res += $"\"from\":\"{From}\",";
            if (!string.IsNullOrEmpty(BackendId))
                res += $"\"backend_id\":\"{BackendId}\",";
            if (!string.IsNullOrEmpty(Backend))
                res += $"\"backend_id\":\"{Backend}\",";
            if (Data.ValueKind != JsonValueKind.Null)
            {
                res += Data + ",";
            }

            // delete trailing ,
            res = res.TrimEnd(',');
            res += "}";
            return res;
        }

        private List<EmailAttachment> Attachments { get; } = new List<EmailAttachment>();

        public void AddAttachment(string fileName)
        {
            Attachments.Add(new EmailAttachment(fileName));
        }

        public void AddAttachment(string fileName, byte[] buffer)
        {
            Attachments.Add(new EmailAttachment(fileName, buffer));
        }

        public void AddAttachment(string fileName, byte[] buffer, string mimeType)
        {
            Attachments.Add(new EmailAttachment(fileName, buffer, mimeType));
        }


        internal MultipartFormDataContent ToFormData(string boundary)
        {
            var form = new MultipartFormDataContent(boundary);
            form.Headers.Remove("Content-Type");
            form.Headers.Add("Content-Type", "multipart/form-data;boundary=" + boundary);

            var s = ToJson();
            var stringContent = new StringContent(s);
            stringContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "message"
            };
            form.Add(stringContent, "message");

            var disp = new StringContent(Disposition.ToString());
            disp.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "disposition"
            };
            form.Add(disp, "disposition");

            foreach (var attachment in Attachments)
            {
                var ba = new ByteArrayContent(attachment.FileContent);
                ba.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "attachments[]",
                    FileName = Path.GetFileName(attachment.Filename)
                };
                ba.Headers.ContentType = new MediaTypeHeaderValue(attachment.MimeType);
                form.Add(ba, "attachments[]", Path.GetFileName(attachment.Filename));
            }


            //  form.ReadAsStringAsync().Result;
            return form;
        }
        internal bool HasAttachments => Attachments.Count > 0;
    }

}
