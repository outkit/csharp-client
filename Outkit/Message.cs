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

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("data")]
        public JsonElement Data { get; set; }
        [JsonPropertyName("backend_id")]
        public string BackendId { get; set; }
        [JsonPropertyName("backend")]
        public string Backend { get; set; }
        [JsonIgnore]
        public MessageDisposition Disposition { get; set; }
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

            var s = JsonSerializer.Serialize(this);

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
                form.Add(ba, "attachments[]", Path.GetFileName(attachment.Filename) ?? string.Empty);
            }

            return form;
        }
        internal bool HasAttachments => Attachments.Count > 0;
    }
}
