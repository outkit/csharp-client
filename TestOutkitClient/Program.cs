using System.Text.Json;
using Outkit;

namespace TestOutkitClient
{
    class Program
    {
        static void Main()
        {
            var client = new Client("Your Key", "Your Pass phrase");
            var resp = client.GetAllMessages();
            // Do something with the list of messages

            // ReSharper disable once NotAccessedVariable
            var res = client.GetMessage(resp[0].Id);
            // Do something with the message

            var msg = new Message
            {
                Type = MessageType.email,
                Project = "test",
                Template = "test",
                Subject = "This is a Test",
                To = "receiver's address",
                From = "sender's address",
                Data = JsonDocument.Parse("{\"name\": \"John Doe\"}").RootElement,
                Disposition = MessageDisposition.Attachment
            };
            msg.AddAttachment("d:\\ExampleFileToSend.pdf");
            // ReSharper disable once RedundantAssignment
            res = client.CreateMessage(msg);
            // Check the response is correct
        }
    }
}