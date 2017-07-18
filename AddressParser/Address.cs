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

namespace Dash.AddressParser
{
    public class Address
    {
        public string FullAddress { get; set; }
        public string Country { get; set; }
        public string Neighborhood { get; set; }
        public string Locality { get; set; }
        public string SubLocality { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string PostalCodeSuffix { get; set; }
        
        /// <summary>
        /// Indicates a n-order civil entity below the country level. 
        /// Within the United States, these administrative levels are states. Not all nations exhibit these administrative levels.
        /// <para>
        /// For example, within the United States, level 1 areas are states and level 2 areas are counties.
        /// </para>
        /// </summary>
        public Dictionary<int, string> AdministrativeArea { get; set; }
    }
}