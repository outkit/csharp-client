## Outkit C# API client
This is the official [C#](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/) client for 
the [Outkit](https://outkit.io/) [API](https://docs.outkit.io/). 

## Contributors
The initial version of this client was created by [Keith Giddings](https://github.com/krgiddings). In later versions, [Tor Erik Dahlsrud](https://github.com/dahlsrud) made significant contributions.

## Questions? Comments?
Feel free to [create a GitHub issue](https://github.com/outkit/csharp-client/issues) with any questions or 
comments you might have about an API client. If you want to contact us less publicly, you can find the most 
current ways of doing so on [our web page](https://outkit.io/contact).

## Getting started
- On outkit.io:
  - Create an Outkit account 
  - Create an API key (you'll here also get a passphrase and secret)
  - Define a Project, Backend and optionally a Template
- Add nuget source https://nuget.pkg.github.com/outkit/index.json
  - This is a public package 
  - Register with your Github credentials
- Add nuget package to your project: OutkitCSharpClient
- Code snippet to send an email: 

```csharp
using Outkit;
//
var message = new Message();
try
{
    message.Type = MessageType.email;
    message.Project = "project";    // this must match the Project identifier in the outkit setup
    message.From = "from@email.address;  // If using SES as backend, the domain of the email address must match your SES configuration
    message.To = "to@email.address";  // any destination email
    message.Subject = "Hello world from Outkit C# Client";
    
    // You can supply a Text or a Html body. If both are defined, html is used:
    //message.HtmlBody = "<body><h1>Hello World!</h1></body>";
    message.TextBody = "Hello World!";
    
    var client = new OutkitClient("outkit-key", "outkit-passphrase", "outkit-secret");
    var result = client.CreateMessage(message);
    
    // If message.Sync == false (like here), result.status will be Received
    // If message.Sync == true, result.status will be Delivered (if everything went well of course)
    Assert.True(result.Status is OutkitStatus.Received or OutkitStatus.Delivered);
}
catch (Exception e)
{
    Console.WriteLine($"Error, message was not sent:\n{e.Message}");
}
```