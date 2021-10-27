using System;
using System.IO;
using System.Linq;
using Outkit;
using Xunit;
using Xunit.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestOutkit
{
    public class UnitTest1
    {
        private readonly string _outkitKey;
        private readonly string _outkitPassphrase;
        private readonly string _outkitSecret;
        private readonly string _outkitFrom;
        private readonly string _outkitTo;
        private readonly string _outkitProject;
        private readonly string _outkitTemplate;

        private readonly ITestOutputHelper _output;
        
        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
            
            // You must first define the following environment variables and restart your IDE:
            _outkitKey = Environment.GetEnvironmentVariable("OutkitKey");
            _outkitPassphrase = Environment.GetEnvironmentVariable("OutkitPassphrase");
            _outkitSecret = Environment.GetEnvironmentVariable("OutkitSecret");
            _outkitFrom = Environment.GetEnvironmentVariable("OutkitFrom");
            _outkitTo = Environment.GetEnvironmentVariable("OutkitTo");
            _outkitProject = Environment.GetEnvironmentVariable("OutkitProject");
            _outkitTemplate = Environment.GetEnvironmentVariable("OutkitTemplate");
            
            if (string.IsNullOrEmpty(_outkitKey) || string.IsNullOrEmpty(_outkitPassphrase)|| 
                string.IsNullOrEmpty(_outkitSecret) || string.IsNullOrEmpty(_outkitFrom) ||
                string.IsNullOrEmpty(_outkitTo) || string.IsNullOrEmpty(_outkitProject) ||
                string.IsNullOrEmpty(_outkitTemplate))
            {
                throw new Exception("Missing Outkit environment variables. Please define and re-run");
            }
        }            
        
        [Fact]
        public void TestHelloWorld()
        {
            var message = new Message();
            try
            {
                message.Type = MessageType.email;
                message.Project = _outkitProject;    // this must match the Project identifier in the outkit setup
                message.From = _outkitFrom;  // If using SES as backend, the domain of the email address must match your SES configuration
                message.To = _outkitTo;
                message.Subject = "Hello world from Outkit C# Client";
                
                // You can supply a Text or a Html body. If both are defined, html is used:
                //message.HtmlBody = $"<body><h1>Hello, time was {DateTime.Now}</h1></body>";
                message.TextBody = $"Text: Hello, time was {DateTime.Now}";
                
                var client = new OutkitClient(_outkitKey, _outkitPassphrase, _outkitSecret);
                var result = client.CreateMessage(message);
                
                // If message.Sync == false (like here), result.status will be Received
                // If message.Sync == true, result.status will be Delivered (if everything went well of course)
                Assert.True(result.Status is OutkitStatus.Received or OutkitStatus.Delivered);

                LogResult(result);
            }
            catch (Exception e)
            {
                _output.WriteLine($"Error, message was not sent:\n{e.Message}");
            }
        }



        [Fact]
        public void TestWithAttachment()
        {
            var message = new Message();
            try
            {
                message.Type = MessageType.email;
                message.Project = _outkitProject;
                message.From = _outkitFrom;
                message.To = _outkitTo;
                message.Subject = "Test sending Outkit";
                message.HtmlBody = $"<body><h1>Hello, time was {DateTime.Now}</h1></body>";
                message.Disposition = MessageDisposition.Attachment;
                
                // Insert one attachment file:
                var fileName = Path.Combine(Path.GetTempPath(), "attachment.txt");
                File.WriteAllText(fileName, "This is a text file\nwith two lines");
                message.AddAttachment(fileName);
                
                // Send
                var client = new OutkitClient(_outkitKey, _outkitPassphrase, _outkitSecret); 
                var result = client.CreateMessage(message);
                LogResult(result);
                
                Assert.True(result.Status is OutkitStatus.Received or OutkitStatus.Delivered);
            }
            catch (Exception e)
            {
                _output.WriteLine($"Error, message was not sent:\n{e.Message}");
            }
            
        }

        
        [Fact]
        public void TestWithTemplate()
        {
            var message = new Message();
            try
            {
                message.Type = MessageType.email;
                message.Project = _outkitProject;
                message.From = _outkitFrom;
                message.To = _outkitTo;
                message.Subject = "Test sending Outkit";
                message.Template = _outkitTemplate;
                message.Data = JsonDocument.Parse("{ \"product_name\":\"Acme Inc\", \"word1\": \"Hello World\" }").RootElement;
                
                var client = new OutkitClient(_outkitKey, _outkitPassphrase, _outkitSecret);
                var result = client.CreateMessage(message);
                LogResult(result);
                Assert.True(result.Status is OutkitStatus.Received or OutkitStatus.Delivered);
            }
            catch (Exception e)
            {
                _output.WriteLine($"Error, message was not sent:\n{e.Message}");
            }
        }
        

        [Fact]
        private void TestGetAllMessages()
        {
            var client = new OutkitClient(_outkitKey, _outkitPassphrase, _outkitSecret);
            var result = client.GetAllMessages();

            _output.WriteLine($"There are {result.Count} messages in Outkit");
            
            // Do something with the list of messages:
            foreach (var message in result.Take(15))
            {
                _output.WriteLine($"{message.ReceivedAt} {message.From} {message.To}");
            }
        }

        
        private void LogResult(OutkitResponse result)
        {
            var jsonOptions = new JsonSerializerOptions
                { 
                    WriteIndented = true, 
                    Converters = { new JsonStringEnumConverter() } };
            var json = JsonSerializer.Serialize(result, jsonOptions);
            _output.WriteLine(json);
        }
    }
}