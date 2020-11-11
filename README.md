## Outkit C# API client
This will become the official [C#](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/) client for 
the [Outkit](https://outkit.io/) [API](https://docs.outkit.io/). 

Unfortunately, we haven’t gotten around to building it yet, but it’s one of the languages we wish to 
support as early as possible.

## Contributing
If you wish to contribute to this client being built sooner rather than later, we very much welcome 
[Pull Requests](https://github.com/outkit/csharp-client/pulls). 
Please check the status of any pending PRs before submitting your own initial implementation, though - 
there may be one in the pipeline already.

We will give full credits in this README (and other suitable places) to any initial contributors.

## Anatomy of an Outkit client library
Please refer to [the source code of the JavaScript client](https://github.com/outkit/javascript-client) 
as a reference implementation. Generally, a client library needs to do the following:

* Provide idiomatic methods that correspond to our API endpoints/verbs
* Translate input data as needed
* Add the correct authentication headers
* Sign the request payload
* Communicate with our API endpoints
* Present the results in an idiomatic way 

The JSON objects that our API returns should probably be automatically converted to the most
natural data type for the language in question, without any manual handling of individual fields. 
This lets us expand the API with new fields without having to update all client libraries.

Languages that support an async model should offer a way to take advantage of this when waiting for a
reply from our API.

## Questions? Comments?
Feel free to [create a GitHub issue](https://github.com/outkit/csharp-client/issues) with any questions or 
comments you might have about an API client. If you want to contact us less publicly, you can find the most 
current ways of doing so on [our web page](https://outkit.io/contact).
