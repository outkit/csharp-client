using System;
using System.Text.Json;

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

        public OutkitResponse(JsonElement elem)
        {
            HtmlTemplateBody = GetStringValue("html_template_body", elem);
            Id = GetStringValue("id", elem);
            To = GetStringValue("to", elem);
            Backend = GetStringValue("backend", elem);
            IdFromSubmitter = GetStringValue("id_from_submitter", elem);
            BackendId = GetStringValue("backend_id", elem);
            Subject = GetStringValue("subject", elem);
            FailedAt = GetDateTime("failed_at", elem);
            TemplateStyles = GetStringValue("template_styles", elem);
            From = GetStringValue("from", elem);
            TextTemplateBody = GetStringValue("template_body", elem);
            Test = GetBooleanValue("test", elem);
            Data = GetElement("data", elem);
            Template = GetStringValue("template", elem);
            TemplateId = GetStringValue("template_id", elem);
            RenderOnly = GetBooleanValue("render_only", elem);
            HtmlBody = GetStringValue("html_body", elem);
            IdFromBackend = GetStringValue("id_from_backend", elem);
            StatusMessage = GetStringValue("status_message", elem);
            TextBody = GetStringValue("text_body", elem);
            PublicId = GetStringValue("public_id", elem);
            QueuedForDeliveryAt = GetDateTime("queued_for_delivery_at", elem);
            Sync = GetBooleanValue("sync", elem);
            ProjectId = GetStringValue("project_id", elem);
            RenderedAt = GetDateTime("rendered_at", elem);
            ReceivedAt = GetDateTime("received_at", elem);
            DeliveredAt = GetDateTime("delivered_at", elem);
            Done = GetBooleanValue("done", elem);
            ProviderId = GetStringValue("provider_id", elem);
            Status = GetStatus(elem);
            QueuedForRenderingAt = GetDateTime("queued_for_rendering_at", elem);
            Project = GetStringValue("project", elem);
            BackendResponse = GetElement("backend_response", elem);

            if (elem.TryGetProperty("type", out var t))
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
