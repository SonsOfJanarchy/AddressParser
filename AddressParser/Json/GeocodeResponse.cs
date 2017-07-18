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
using Dash.AddressParser.Exceptions;
using Newtonsoft.Json;

namespace Dash.AddressParser.Json
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GeocodeResponse
    {
        [JsonObject(MemberSerialization.OptIn)]
        public class GeocodeResult
        {
            [JsonObject(MemberSerialization.OptIn)]
            public class AddressComponent
            {
                [JsonProperty("long_name")]
                public string LongName { get; set; }

                [JsonProperty("short_name")]
                public string ShortName { get; set; }

                [JsonProperty("types")]
                public List<string> Types { get; set; }

                public string GetName(bool useLongName)
                {
                    return useLongName ? LongName : ShortName;
                }
            }

            [JsonProperty("address_components")]
            public List<AddressComponent> AddressComponents { get; set; }

            [JsonProperty("formatted_address")]
            public string FormattedAddress { get; set; }

            public Address ToAddress(bool useLongNames = true)
            {
                return new Address()
                {
                    FullAddress = FormattedAddress,
                    Country = AddressComponents.FirstOrDefault(component => component.Types.Contains("country"))?.GetName(useLongNames),
                    PostalCode = AddressComponents.FirstOrDefault(component => component.Types.Contains("postal_code"))?.GetName(useLongNames),
                    PostalCodeSuffix = AddressComponents.FirstOrDefault(component => component.Types.Contains("postal_code_suffix"))?.GetName(useLongNames),
                    Neighborhood = AddressComponents.FirstOrDefault(component => component.Types.Contains("neighborhood"))?.GetName(useLongNames),
                    Locality = AddressComponents.FirstOrDefault(component => component.Types.Contains("locality"))?.GetName(useLongNames),
                    SubLocality = AddressComponents.FirstOrDefault(component => component.Types.Contains("sublocality"))?.GetName(useLongNames),
                    Street = AddressComponents.FirstOrDefault(component => component.Types.Contains("route"))?.GetName(useLongNames),
                    StreetNumber = AddressComponents.FirstOrDefault(component => component.Types.Contains("street_number"))?.GetName(useLongNames),

                    AdministrativeArea = new Dictionary<int, string>()
                    {
                        { 1, AddressComponents.FirstOrDefault(component => component.Types.Contains("administrative_area_level_1"))?.GetName(useLongNames) },
                        { 2, AddressComponents.FirstOrDefault(component => component.Types.Contains("administrative_area_level_2"))?.GetName(useLongNames) },
                        { 3, AddressComponents.FirstOrDefault(component => component.Types.Contains("administrative_area_level_3"))?.GetName(useLongNames) },
                        { 4, AddressComponents.FirstOrDefault(component => component.Types.Contains("administrative_area_level_4"))?.GetName(useLongNames) },
                        { 5, AddressComponents.FirstOrDefault(component => component.Types.Contains("administrative_area_level_5"))?.GetName(useLongNames) }
                    }
                };
            }
        }

        [JsonProperty("results")]
        public List<GeocodeResult> Results { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

        public GeocodeResponse EnsureSuccessStatusCode()
        {
            switch (Status)
            {
                case "OK": return this;
                case "ZERO_RESULTS": throw new AddressNotFoundException(ErrorMessage);
                case "OVER_QUERY_LIMIT": throw new ApiQuotaExceededException(ErrorMessage);
                case "REQUEST_DENIED": throw new InvalidApiKeyException(ErrorMessage);
                case "INVALID_REQUEST": throw new InvalidApiRequestException(ErrorMessage);
                default: throw new ApiUnknownException(ErrorMessage);
            }
        }
    }
}