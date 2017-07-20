#region License
// ========================================================================
// Copyright © 2017 Alexandre Quoniou
// 
// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ========================================================================
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dash.AddressParser.Exceptions;
using Newtonsoft.Json;

namespace Dash.AddressParser.Json
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BingResponse
    {
        [JsonObject(MemberSerialization.OptIn)]
        public class ResourceSet
        {
            [JsonObject(MemberSerialization.OptIn)]
            public class Resource
            {
                public class BingAddress
                {
                    private static readonly Regex AddressLineRegex = new Regex(@"^(\d+)\s", RegexOptions.Compiled);

                    [JsonProperty("addressLine")]
                    public string AddressLine { get; set; }

                    [JsonProperty("adminDistrict")]
                    public string AdminDistrict { get; set; }

                    [JsonProperty("adminDistrict2")]
                    public string AdminDistrict2 { get; set; }

                    [JsonProperty("adminDistrict3")]
                    public string AdminDistrict3 { get; set; }

                    [JsonProperty("adminDistrict4")]
                    public string AdminDistrict4 { get; set; }

                    [JsonProperty("countryRegion")]
                    public string CountryRegion { get; set; }

                    [JsonProperty("formattedAddress")]
                    public string FormattedAddress { get; set; }

                    [JsonProperty("locality")]
                    public string Locality { get; set; }

                    [JsonProperty("postalCode")]
                    public string PostalCode { get; set; }

                    public Address ToAddress()
                    {
                        string street, streetNumber;

                        var addressLineMatch = AddressLineRegex.Match(AddressLine ?? string.Empty);
                        if (addressLineMatch.Success)
                        {
                            street = AddressLine?.Remove(addressLineMatch.Index, addressLineMatch.Length);
                            streetNumber = addressLineMatch.Groups[1].Value;
                        }
                        else
                        {
                            street = AddressLine;
                            streetNumber = string.Empty;
                        }

                        return new Address()
                        {
                            FullAddress = FormattedAddress,
                            Country = CountryRegion,
                            PostalCode = PostalCode,
                            PostalCodeSuffix = string.Empty,
                            Neighborhood = string.Empty,
                            Locality = Locality,
                            SubLocality = string.Empty,
                            Street = street,
                            StreetNumber = streetNumber,

                            AdministrativeArea = new Dictionary<int, string>()
                            {
                                { 1, AdminDistrict },
                                { 2, AdminDistrict2 },
                                { 3, AdminDistrict3 },
                                { 4, AdminDistrict4 },
                                { 5, string.Empty }
                            }
                        };
                    }
                }

                [JsonProperty("address")]
                public BingAddress Address { get; set; }
            }

            [JsonProperty("estimatedTotal")]
            public int EstimatedTotal { get; set; }

            [JsonProperty("resources")]
            public List<Resource> Resources { get; set; }
        }

        [JsonProperty("resourceSets")]
        public List<ResourceSet> ResourceSets { get; set; }

        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("statusDescription")]
        public string StatusDescription { get; set; }

        public BingResponse EnsureSuccessStatusCode()
        {
            switch (StatusCode)
            {
                case 200:
                    if (ResourceSets.Any(set => set.EstimatedTotal == 0)) throw new AddressNotFoundException(StatusDescription);
                    return this;
                default: throw new ApiUnknownException(StatusDescription);
            }
        }
    }
}