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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dash.AddressParser.Exceptions;
using Dash.AddressParser.Json;
using Newtonsoft.Json;

namespace Dash.AddressParser
{
    /// <summary>
    /// This class allows one to parse addresses using Bing Maps API. 
    /// However, it is important to note that this API is not the most precise of all, and data loss occurs frequently (street numbers are often lost).
    /// </summary>
    public class BingAddressParser : IAddressParser
    {
        private readonly HttpClient _client;
        public string ApiKey { get; set; }

        public BingAddressParser(string apiKey)
        {
            _client = new HttpClient() { BaseAddress = new Uri("https://dev.virtualearth.net/") };
            ApiKey = apiKey;
        }

        public async Task<IEnumerable<Address>> ParseAsync(string address)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException(nameof(address));

            var response = await _client.GetAsync($"REST/v1/Locations?key={ApiKey}&query={WebUtility.UrlEncode(address)}");

            if (response.StatusCode == HttpStatusCode.Unauthorized) throw new InvalidApiKeyException();
            response.EnsureSuccessStatusCode();

            var bingData = JsonConvert.DeserializeObject<BingResponse>(await response.Content.ReadAsStringAsync());
            bingData.EnsureSuccessStatusCode();

            return bingData.ResourceSets.Select(set => set.Resources.Select(resource => resource.Address.ToAddress())).SelectMany(m => m);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
