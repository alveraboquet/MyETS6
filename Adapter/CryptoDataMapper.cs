using ProviderContract.Data.Entities;
using SourceEts.Models.TimeFrameTransformModel;
using SourceEts.Table;
using System;
using System.Collections.Generic;

namespace Adapter
{
    public static class CryptoDataMapper
    {
        private static readonly DateTime unix = new DateTime(1970, 1, 1);

        public static Glass MapOrderBook(OrderBook orderBook)
        {
            Glass glass = new Glass()
            {
                ClassCode = orderBook.ClassCode,
                Deep = orderBook.Asks.Count,
                Symbol = orderBook.Pair
            };

            foreach (var ask in orderBook.Asks)
            {
                glass.QuotationsSell.Add(new GlassQuotation()
                {
                    SellQty = ask.Quantity,
                    Price = ask.Price
                });
            }

            foreach (var bid in orderBook.Bids)
            {
                glass.QuotationsBuy.Add(new GlassQuotation()
                {
                    BuyQty = bid.Quantity,
                    Price = bid.Price
                });
            }

            return glass;
        }

        public static Candles MapKlines(List<Kline> klines)
        {
            Candles candlesData = new Candles();

            foreach (var kline in klines)
            {
                candlesData.Candle.Add(new CandleModel()
                {
                    TradeDateTime = unix.AddSeconds(kline.OpenTime),
                    Open = kline.Open,
                    High = kline.High,
                    Low = kline.Low,
                    Close = kline.Close,
                    Volume = kline.Volume
                });
            }

            return candlesData;
        }
    }
}
