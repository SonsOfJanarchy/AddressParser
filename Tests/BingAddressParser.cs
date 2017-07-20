using System;
using System.Linq;
using Dash.AddressParser.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dash.AddressParser.Tests
{
    [TestClass]
    public class BingAddressParserTests
    {
        // Get your Bing Maps API key here (https://www.bingmapsportal.com).
        // Do NOT forget to remove it before a commit !
        private static readonly string ApiKey = "";

        [TestMethod]
        public void ParseAsync_PartialDataLoss()
        {
            var address = "3947 W CLAREMONT ST PHOENIX 85019-1415";

            using (IAddressParser parser = new BingAddressParser(ApiKey))
            {
                var result = parser.ParseAsync(address).Result.FirstOrDefault();
                Assert.AreEqual(result.StreetNumber, "3947", true);
                Assert.AreEqual(result.Street, "W Claremont St", true);
                Assert.AreEqual(result.Locality, "Phoenix", true);
                Assert.AreEqual(result.PostalCode, "85019", true);
                Assert.AreEqual(result.PostalCodeSuffix, string.Empty, true);
            }
        }

        [TestMethod]
        public void ParseAsync_DataLoss()
        {
            var address = "Champ de Mars, 5 Avenue Anatole France, 75007 Paris";

            using (IAddressParser parser = new BingAddressParser(ApiKey))
            {
                var result = parser.ParseAsync(address).Result.FirstOrDefault();
                Assert.AreEqual(result.StreetNumber, string.Empty, true);
                Assert.AreEqual(result.Street, "Champ-de-Mars", true);
                Assert.AreEqual(result.Locality, "Paris", true);
                Assert.AreEqual(result.PostalCode, "75007", true);
            }
        }

        [TestMethod]
        public void ParseAsync_RandomInput()
        {
            var address = "fiupohbfuipzeobnhfupiodskeyboardmashiophfseiufhisjfpsdijo";
            
            var exception = Assert.ThrowsException<AggregateException>(() =>
            {
                using (IAddressParser parser = new BingAddressParser(ApiKey))
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
                using (IAddressParser parser = new BingAddressParser("abcdefghijklmnopqrstuvwxyz"))
                {
                    parser.ParseAsync(address).Wait();
                }
            });

            Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidApiKeyException));
        }
    }
}
