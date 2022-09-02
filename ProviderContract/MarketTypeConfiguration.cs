using System;

namespace ProviderContract
{
    [Serializable]
    public class MarketTypeConfiguration
    {
        public string MarketName;
        public ApiType ApiType { get; set; }
        public string BaseURL;

        public string PublicWss;
        public string PrivateWss;
        public string BaseWss;

        public string WEBSOCKET_ACCESS_REQUEST_ENDPOINT;

        public string GET_AGGREGATED_TRADES;
        public string GET_BOOKTICKER_ENDPOINT;
        public string GET_KLINES_ENDPOINT;
        public string GET_ORDER_BOOK_ENDPOINT;
        public string GET_PAIRS_ENDPOINT;
        public string GET_SERVER_TIME_ENDPOINT;
        public string GET_TICKERS_ENDPOINT;
        public string GET_TICKS_ENDPOINT;

        public string GET_ALL_ORDERS_ENDPOINT;
        public string GET_ACTIVE_ORDERS_ENDPOINT;
        public string GET_CANCELLED_ORDERS_ENDPOINT;
        public string GET_FILLED_ORDERS_ENDPOINT;

        public string GET_BALANCE_ENDPOINT;
        public string GET_TRADES_ENDPOINT;

        public string CANCEL_AN_ORDER_ENDPOINT;
        public string PLACE_ORDER_ENDPOINT;

        public string AGG_TRADES_CHANNEL;
        public string BOOKTICKER_CHANNEL;
        public string KLINES_CHANNEL;
        public string ORDER_BOOK_CHANNEL;
        public string TICKERS_CHANNEL;
        public string TICKS_CHANNEL;

        public string BALANCE_CHANNEL;
        public string ORDERS_CHANNEL;
        public string TRADES_CHANNEL;

    }
}
