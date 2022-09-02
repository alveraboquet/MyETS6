using Bybit.Net.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS6_Tests.BybitConnectorTests
{
    public class BybitRestTests
    {
        private BybitClient _bybitClient;

        [SetUp]
        public void Setup()
        {
            _bybitClient = new BybitClient();
        }

        [Test]
        public void GetUsdFuturesTickersAsync_Test()
        {


            Assert.Pass();
        }
    }
}
