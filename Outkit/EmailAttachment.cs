using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace Outkit
{
    public class EmailAttachment
    {
        public string Filename { get; set; }
        public byte[] FileContent { get; set; }
        public string MimeType { get; set; }

        private static string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

        private void Init(string fileName, byte[] buffer, string mimeType)
        {
            Filename = fileName;
            FileContent = buffer;
            MimeType = mimeType;
        }

        public EmailAttachment(string fileName)
        {
            if (File.Exists(fileName))
            {
                Init(fileName, File.ReadAllBytes(fileName), GetContentType(fileName));
            }
        }

        public EmailAttachment(string fileName, byte[] buffer)
        {
            Init(fileName, buffer, GetContentType(fileName));
        }

        public EmailAttachment(string fileName, byte[] buffer, string mimeType)
        {
            Init(fileName, buffer, mimeType);
        }
    }
}
