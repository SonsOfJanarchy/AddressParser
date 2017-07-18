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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dash.AddressParser.Exceptions;
using Dash.AddressParser.Json;
using Newtonsoft.Json;

namespace Dash.AddressParser
{
    public class GoogleAddressParser : IAddressParser
    {
        private readonly HttpClient _client;
        public string ApiKey { get; set; }
        public CultureInfo Culture { get; set; }

        public GoogleAddressParser(string apiKey) : this(apiKey, CultureInfo.CurrentCulture)
        {
        }

        public GoogleAddressParser(string apiKey, CultureInfo culture)
        {
            _client = new HttpClient() { BaseAddress = new Uri("https://maps.googleapis.com/") };
            ApiKey = apiKey;
            Culture = culture;
        }

        public Task<IEnumerable<Address>> ParseAsync(string address)
        {
            return ParseAsync(address, true);
        }

        public async Task<IEnumerable<Address>> ParseAsync(string address, bool useLongNames)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException(nameof(address));

            var response = await _client.GetAsync($"maps/api/geocode/json?key={ApiKey}&language={Culture.Name}&address={WebUtility.UrlEncode(address)}");
            response.EnsureSuccessStatusCode();

            var geocodeData = JsonConvert.DeserializeObject<GeocodeResponse>(await response.Content.ReadAsStringAsync());
            geocodeData.EnsureSuccessStatusCode();

            if (geocodeData.Results.Count == 0)
            {
                throw new AddressNotFoundException(geocodeData.ErrorMessage);
            }

            return geocodeData.Results.Select(result => result.ToAddress(useLongNames));
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
