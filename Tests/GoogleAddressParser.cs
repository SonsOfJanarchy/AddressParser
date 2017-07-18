using System;
using System.Linq;
using Dash.AddressParser.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dash.AddressParser.Tests
{
    [TestClass]
    public class GoogleAddressParserTests
    {
        // Fill your Google APIs key here (https://console.developers.google.com).
        // Do NOT forget to remove it before a commit !
        // It seems to be working without an api key too, so you might not need to bother about that.
        private static readonly string ApiKey = "";

        [TestMethod]
        public void ParseAsync_NoDataLoss()
        {
            var address = "3947 W CLAREMONT ST PHOENIX 85019-1415";

            using (IAddressParser parser = new GoogleAddressParser(ApiKey))
            {
                var result = parser.ParseAsync(address).Result.FirstOrDefault();
                Assert.AreEqual(result.StreetNumber, "3947", true);
                Assert.AreEqual(result.Street, "West Claremont Street", true);
                Assert.AreEqual(result.Locality, "Phoenix", true);
                Assert.AreEqual(result.PostalCode, "85019", true);
                Assert.AreEqual(result.PostalCodeSuffix, "1415", true);
            }
        }

        [TestMethod]
        public void ParseAsync_RandomInput()
        {
            var address = "My name is Bond, James Bond (but is that sentence really random ?).";
            
            var exception = Assert.ThrowsException<AggregateException>(() =>
            {
                using (IAddressParser parser = new GoogleAddressParser(ApiKey))
                {
                    parser.ParseAsync(address).Wait();
                }
            });
            Assert.IsInstanceOfType(exception.InnerException, typeof(AddressNotFoundException));
            
        }

        [TestMethod]
        public void ParseAsync_WrongApiKey()
        {
            var address = "Champ de Mars, 5 Avenue Anatole France, 75007 Paris";
            
            var exception = Assert.ThrowsException<AggregateException>(() =>
            {
                using (IAddressParser parser = new GoogleAddressParser("abcdefghijklmnopqrstuvwxyz"))
                {
                    parser.ParseAsync(address).Wait();
                }
            });
            Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidApiKeyException));
        }
    }
}
