using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Outkit
{
    public class OutkitResponse
    {
        public string HtmlTemplateBody { get; set; }
        public string Id { get; set; }
        public string To { get; set; }
        public string Backend { get; set; }
        public string IdFromSubmitter { get; set; }
        public MessageType Type { get; set; }
        public string BackendId { get; set; }
        public string Subject { get; set; }
        public DateTime FailedAt { get; set; }
        public string TemplateStyles { get; set; }
        public string From { get; set; }
        public string TextTemplateBody { get; set; }
        public bool Test { get; set; }
        [JsonIgnore()]
        public JsonElement Data { get; set; }
        public string Template { get; set; }
        public string TemplateId { get; set; }
        public bool RenderOnly { get; set; }
        public string HtmlBody { get; set; }
        public string IdFromBackend { get; set; }
        public string StatusMessage { get; set; }
        public string TextBody { get; set; }
        public string PublicId { get; set; }
        public DateTime QueuedForDeliveryAt { get; set; }
        public bool Sync { get; set; }
        public string ProjectId { get; set; }
        public DateTime RenderedAt { get; set; }
        public DateTime ReceivedAt { get; set; }
        public DateTime DeliveredAt { get; set; }
        public bool Done { get; set; }
        public string ProviderId { get; set; }
        public OutkitStatus Status { get; set; }
        public DateTime QueuedForRenderingAt { get; set; }
        public string Project { get; set; }
        public JsonElement BackendResponse { get; set; }

        public OutkitResponse(JsonElement data)
        {
                HtmlTemplateBody = GetStringValue("html_template_body", data);
                Id = GetStringValue("id", data);
                To = GetStringValue("to", data);
                Backend = GetStringValue("backend", data);
                IdFromSubmitter = GetStringValue("id_from_submitter", data);
                BackendId = GetStringValue("backend_id", data);
                Subject = GetStringValue("subject", data);
                FailedAt = GetDateTime("failed_at", data);
                TemplateStyles = GetStringValue("template_styles", data);
                From = GetStringValue("from", data);
                TextTemplateBody = GetStringValue("template_body", data);
                Test = GetBooleanValue("test", data);
                Data = GetElement("data", data);
                Template = GetStringValue("template", data);
                TemplateId = GetStringValue("template_id", data);
                RenderOnly = GetBooleanValue("render_only", data);
                HtmlBody = GetStringValue("html_body", data);
                IdFromBackend = GetStringValue("id_from_backend", data);
                StatusMessage = GetStringValue("status_message", data);
                TextBody = GetStringValue("text_body", data);
                PublicId = GetStringValue("public_id", data);
                QueuedForDeliveryAt = GetDateTime("queued_for_delivery_at", data);
                Sync = GetBooleanValue("sync", data);
                ProjectId = GetStringValue("project_id", data);
                RenderedAt = GetDateTime("rendered_at", data);
                ReceivedAt = GetDateTime("received_at", data);
                DeliveredAt = GetDateTime("delivered_at", data);
                Done = GetBooleanValue("done", data);
                ProviderId = GetStringValue("provider_id", data);
                Status = GetStatus(data);
                QueuedForRenderingAt = GetDateTime("queued_for_rendering_at", data);
                Project = GetStringValue("project", data);
                BackendResponse = GetElement("backend_response", data);

                if (data.TryGetProperty("type", out var t))
                    Type = t.GetString() == "sms" ? MessageType.sms : MessageType.email;
        }
        private static OutkitStatus GetStatus(JsonElement elem)
        {
            return GetStringValue("status", elem) switch
            {
                "received" => OutkitStatus.Received,
                "queued_for_rendering" => OutkitStatus.QueuedForRendering,
                "rendered" => OutkitStatus.Rendered,
                "queued_for_delivery" => OutkitStatus.QueuedForDelivery,
                "delivered" => OutkitStatus.Delivered,
                "render_error" => OutkitStatus.RenderError,
                "backend_error" => OutkitStatus.BackendError,
                "internal_error" => OutkitStatus.InternalError,
                _ => OutkitStatus.InternalError
            };
        }
        private static DateTime GetDateTime(string property, JsonElement elem)
        {
            var value = GetElement(property, elem);
            if (value.ValueKind != JsonValueKind.String) return default;
            return value.TryGetDateTime(out var res) ? res : default;
        }

        private static JsonElement GetElement(string property, JsonElement elem)
        {
            return elem.TryGetProperty(property, out var value) ? value : default;
        }

        private static string GetStringValue(string property, JsonElement elem)
        {
            var field = GetElement(property, elem);
            return field.ValueKind != JsonValueKind.Undefined ? field.GetString() : default;
        }

        private static bool GetBooleanValue(string property, JsonElement elem)
        {
            var field = GetElement(property, elem);
            return field.ValueKind == JsonValueKind.True;
        }
    }
}
