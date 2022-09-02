using Adapter;
using CryptoCon.Binance;
using CryptoCon.Bitfinex;
using CryptoCon.Bybit;
using CryptoCon.FTX;
using CryptoCon.Huobi;
using Moex.MoexFF;
using System.Collections.Generic;

namespace CryptoCon
{
    public class CfgCryptoCon
    {
        private HuobiClass Huobi = new HuobiClass();
        private Binance.BitfinexConnector Binance = new Binance.BitfinexConnector();
        private BybitConnector Bybit = new BybitConnector();
        private FTXDima80LVL Ftx = new FTXDima80LVL();
        private KucoinConnector KucoinConnector = new KucoinConnector();
        private Bitfinex.BitfinexConnector BitfinexConnector = new Bitfinex.BitfinexConnector();

        private MoexEq moexEq = new MoexEq();
        private MoexCur moexCur = new MoexCur();
        private MoexForts moexForts = new MoexForts();

        public List<AbstractTerminal> Connectors = new List<AbstractTerminal>();

        public void CreateListConnectors()
        {
            Connectors.Add(Huobi);
            Connectors.Add(Binance);
            Connectors.Add(KucoinConnector);
            Connectors.Add(Ftx);
            Connectors.Add(Bybit);
            Connectors.Add(BitfinexConnector);
            //Connectors.Add(moexEq);
            //Connectors.Add(moexCur);
            //Connectors.Add(moexForts);
        }
    }
}
