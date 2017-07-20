# AddressParser

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](http://paypal.me/MysteryDash) [![License](http://img.shields.io/:license-mit-blue.svg)](http://doge.mit-license.org) 

This project is a .NET Standard Library used to parse non standardized addresses into their raw components using various online APIs, just because they make a much better job than using regular expressions or other techniques you could use to decompose an address into its fundamental compounds, even if you have some weirdly formatted addresses.

## Getting started

You can install this library using the following NuGet command :

        Install-Package Dash.AddressParser
        
Then, you can choose one of the following parsers :
* Google Geocoding API
* Bing Maps API
* More coming soon...

## Example

```csharp
using (IAddressParser parser = new GoogleAddressParser(ApiKey))
{
    IEnumerable<Address> results = await parser.ParseAsync(address);
}
```

It's really that simple. Of course, you should probably handle the exceptions thrown by the parser.

## What has yet to be done

There might not be much to say around about this, except that adding new APIs would be great, or write a bit of documentation.

## Contributing

Feel free to contribute and send a pull request with what you made.  
I'll check it out as soon as possible !

## Contributors

The following people are contributors of this project :
- [Alexandre Quoniou / MysteryDash](https://github.com/MysteryDash)

## Versioning

We use [Semantic Versioning 2.0.0](http://semver.org/) for versioning (starting at 0.1.0).  
The current version of this project is version 0.2.0.  
  
About the version, I know it might not be for the best that it is updated manually, but :
* Does it matter that much *for this particular project* ?
* I don't have a definitive answer concerning what tool I should use to update the version automatically anyway. If you have experience with that, feel free to tell me what you use.

## License

This project is licensed under the MIT License.  
See the [LICENSE.md](LICENSE.md) file for details.
