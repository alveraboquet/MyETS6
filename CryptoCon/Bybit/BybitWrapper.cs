using Adapter;
using Adapter.Config;
using Binance.Net.Interfaces;
using Bybit.Net.Enums;
using Bybit.Net.Objects;
using Bybit.Net.Objects.Models;
using Bybit.Net.Objects.Models.Socket;
using Bybit.Net.Objects.Models.Socket.Spot;
using Bybit.Net.Objects.Models.Spot;
using CommonDataContract;
using CommonDataContract.ReactData;
using CryptoCon.Binance;
using CryptoCon.Binance.Models;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Objects;
using SourceEts;
using SourceEts.Models.TimeFrameTransformModel;
using SourceEts.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Input;
using Order = CryptoCon.Binance.Models.Order;

namespace CryptoCon.Bybit
{
    public class BybitWrapper
    {
        public string NameExchange = "Bybit";

        private readonly string _key;
        private readonly string _secret;

        private string _typeMarket;
        private AbstractTerminal _abstractTerminal;
        private BybitSocket _bybitSocket;
        private BybitRest _bybitRest;

        public BybitWrapper(string key, string secret, string typeMarket, AbstractTerminal abstractTerminal)
        {
            _key = key;
            _secret = secret;
            _typeMarket = typeMarket;
            _abstractTerminal = abstractTerminal;
            _bybitSocket = new BybitSocket(new BybitSocketClientOptions() 
            { 
                //SpotStreamsOptions = new BybitSocketApiClientOptions() { BaseAddressAuthenticated = BybitApiAddresses.TestNet.SpotPrivateSocketClientAddress },
                //UsdPerpetualStreamsOptions = new BybitSocketApiClientOptions() { BaseAddressAuthenticated = BybitApiAddresses.TestNet.UsdPerpetualPrivateSocketClientAddress },
                ApiCredentials = new ApiCredentials(_key, _secret)
            });
            _bybitRest = new BybitRest(new BybitClientOptions() 
            {
                //SpotApiOptions = new RestApiClientOptions(BybitApiAddresses.TestNet.SpotRestClientAddress),
                //UsdPerpetualApiOptions = new RestApiClientOptions(BybitApiAddresses.TestNet.UsdPerpetualRestClientAddress),
                ApiCredentials = new ApiCredentials(_key, _secret) 
            });
        }

        public async Task SubscribeToAccountUpdateAsync()
        {
            if (_typeMarket == ConfigTermins.TypeCryptoAccountSpot)
            {
                await _bybitSocket.SubscribeToAccountUpdatesAsync(_typeMarket);

                var tranferToTerminalOrders = new ActionBlock<BybitSpotOrderUpdate>(x =>
                {
                    Orders order = null;

                    if (x.Status.Equals(OrderStatus.New))
                    {
                        order = new Orders
                        {
                            Status = ConfigTermins.Active,
                            Account = _key,
                            ClientCode = _secret,
                            Balance = Convert.ToDouble(x.Quantity),
                            Price = Convert.ToDouble(x.Price),
                            Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                            Number = x.OrderId.ToString(),
                            Time = x.CreateTime,
                            Symbol = x.Symbol.ToUpper(),
                            ClassCode = $"{NameExchange} {_typeMarket}",
                            Quantity = Convert.ToDouble(x.Quantity),
                            Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                        };

                        _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                    }
                    else if (x.Status.Equals(OrderStatus.Canceled))
                    {
                        order = new Orders()
                        {
                            Status = ConfigTermins.Cancel,
                            Account = _key,
                            ClientCode = _secret,
                            Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                            Symbol = x.Symbol.ToUpper(),
                            ClassCode = $"{NameExchange} {_typeMarket}",
                            Time = x.CreateTime,
                            Number = x.OrderId.ToString(),
                            Balance = Convert.ToDouble(x.Quantity),
                            CancelOrderTime = x.CreateTime,
                            Price = Convert.ToDouble(x.Price),
                            Quantity = Convert.ToDouble(x.Quantity - x.TotalQuantityFilled),
                            Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                        };

                        _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                    }
                    else if (x.Status.Equals(OrderStatus.Filled))
                    {
                        var deal = new Deal()
                        {
                            Account = _key,
                            ClientCode = _secret,
                            ClassCode = $"{NameExchange} {_typeMarket}",
                            Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                            Price = Convert.ToDouble(x?.Price),
                            Symbol = x.Symbol,
                            Order = x.OrderId.ToString(),
                            NumberTrade = string.Empty, // TODO: Number???
                            DateTrade = x.CreateTime,
                            Quantity = Convert.ToDouble(x?.Quantity),
                            Volume = Convert.ToDouble(x?.Quantity) * Convert.ToDouble(x?.Price),
                        };

                        order = new Orders()
                        {
                            Status = ConfigTermins.Performed,
                            Account = _key,
                            ClientCode = _secret,
                            Balance = Convert.ToDouble(x.Quantity - x.TotalQuantityFilled),
                            Number = x.OrderId.ToString(),
                            Symbol = x.Symbol,
                            ClassCode = $"{NameExchange} {_typeMarket}"
                        };

                        _abstractTerminal.AddDeal(_abstractTerminal.BaseTable.DealsList, deal);
                        _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                    }
                });

                var transferToBalanceTerminal = new ActionBlock<BybitSpotAccountUpdate>(x =>
                {
                    var shares = x.Balances.Select(x => new MoneyPosition()
                    {
                        ClientCode = _secret,
                        Account = _key,
                        Asset = x.Asset.ToUpper(),
                        Currency = x.Asset.ToUpper(),
                        Balance = Convert.ToDouble(x.Available),
                        Group = NameExchange,
                    }).ToList();

                    foreach (var share in shares)
                    {
                        _abstractTerminal.AddMoneyShare(_abstractTerminal.BaseTable.LimitMoneySharesList, share);
                    }
                });

                _bybitSocket.BufferSpotOrderUpdate.LinkTo(tranferToTerminalOrders);
                _bybitSocket.BufferSpotAccountUpdate.LinkTo(transferToBalanceTerminal);
            }
            else if (_typeMarket == ConfigTermins.TypeCryptoAccountFutures)
            {
                await _bybitSocket.SubscribeToAccountUpdatesAsync(_typeMarket);

                var tranferToTerminalOrders = new ActionBlock<IEnumerable<BybitUsdPerpetualOrderUpdate>>(x =>
                {
                    foreach (BybitUsdPerpetualOrderUpdate orderUpdate in x)
                    {
                        Orders order = null;

                        if (orderUpdate.Status.Equals(OrderStatus.New))
                        {
                            order = new Orders
                            {
                                Status = ConfigTermins.Active,
                                Account = _key,
                                ClientCode = _secret,
                                Balance = Convert.ToDouble(orderUpdate.Quantity),
                                Price = Convert.ToDouble(orderUpdate.Price),
                                Id = orderUpdate.ClientOrderId is null ? string.Empty : orderUpdate.ClientOrderId,
                                Number = orderUpdate.Id.ToString(),
                                Time = orderUpdate.Timestamp,
                                Symbol = orderUpdate.Symbol.ToUpper(),
                                ClassCode = $"{NameExchange} {_typeMarket}",
                                Quantity = Convert.ToDouble(orderUpdate.Quantity),
                                Operation = orderUpdate.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                            };

                            _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                        }
                        else if (orderUpdate.Status.Equals(OrderStatus.Canceled))
                        {
                            order = new Orders()
                            {
                                Status = ConfigTermins.Cancel,
                                Account = _key,
                                ClientCode = _secret,
                                Id = orderUpdate.ClientOrderId is null ? string.Empty : orderUpdate.ClientOrderId,
                                Symbol = orderUpdate.Symbol.ToUpper(),
                                ClassCode = $"{NameExchange} {_typeMarket}",
                                Time = orderUpdate.Timestamp,
                                Number = orderUpdate.Id.ToString(),
                                Balance = Convert.ToDouble(orderUpdate.Quantity),
                                CancelOrderTime = orderUpdate.Timestamp,
                                Price = Convert.ToDouble(orderUpdate.Price),
                                Quantity = Convert.ToDouble(orderUpdate.Quantity),
                                Operation = orderUpdate.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                            };

                            _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                        }
                        else if (orderUpdate.Status.Equals(OrderStatus.Filled))
                        {
                            var deal = new Deal()
                            {
                                Account = _key,
                                ClientCode = _secret,
                                ClassCode = $"{NameExchange} {_typeMarket}",
                                Operation = orderUpdate.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                                Price = Convert.ToDouble(orderUpdate.Price),
                                Symbol = orderUpdate.Symbol,
                                Order = orderUpdate.Id.ToString(),
                                NumberTrade = string.Empty, // TODO: ???
                                DateTrade = orderUpdate.Timestamp,
                                Quantity = Convert.ToDouble(orderUpdate.Quantity),
                                Volume = Convert.ToDouble(orderUpdate.Quantity) * Convert.ToDouble(orderUpdate.Price),
                            };

                            order = new Orders()
                            {
                                Status = ConfigTermins.Performed,
                                Account = _key,
                                ClientCode = _secret,
                                Balance = Convert.ToDouble(orderUpdate.Quantity),
                                Number = orderUpdate.Id.ToString(),
                                Symbol = orderUpdate.Symbol,
                                ClassCode = $"{NameExchange} {_typeMarket}"
                            };

                            _abstractTerminal.AddDeal(_abstractTerminal.BaseTable.DealsList, deal);
                            _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                        }
                    }
                });

                var transferToBalanceTerminal = new ActionBlock<IEnumerable<BybitBalanceUpdate>>(x =>
                {
                    foreach (BybitBalanceUpdate balanceUpdate in x)
                    {
                        MoneyPosition share = new MoneyPosition()
                        {
                            ClientCode = _secret,
                            Account = _key,
                            Asset = balanceUpdate != null && balanceUpdate.Asset != null ? balanceUpdate.Asset.ToUpper() : string.Empty,
                            Balance = Convert.ToDouble(balanceUpdate?.WalletBalance)
                        };

                        _abstractTerminal.AddMoneyShare(_abstractTerminal.BaseTable.LimitMoneySharesList, share);
                    }
                });

                _bybitSocket.BufferUsdFuturesOrderUpdate.LinkTo(tranferToTerminalOrders);
                _bybitSocket.BufferBalanceUpdateUsdFutures.LinkTo(transferToBalanceTerminal);
            }
        }

        public async Task SubcribeToInstrumentsAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            await Task.Run(() =>
            {
                _bybitSocket.SubcribeToInstrumentsAsync(avalibleInstrumentsModel, _typeMarket);

                if (_typeMarket == ConfigTermins.TypeCryptoAccountSpot)
                {
                    var transferTickersToTerminal = new ActionBlock<BybitSpotTickerUpdate>(data =>
                    {
                        Securities securities = new Securities()
                        {
                            ClosePrice = Convert.ToDouble(data.LastPrice),
                            LastPrice = Convert.ToDouble(data.LastPrice),
                            OpenPrice = Convert.ToDouble(data.OpenPrice),
                            MaxPrice = Convert.ToDouble(data.HighPrice),
                            MinPrice = Convert.ToDouble(data.LowPrice),
                            Bid = 0, // TODO: нет таких данных в модели
                            Offer = 0, // TODO: нет таких данных в модели
                            TimeLastChange = DateTime.UtcNow,
                            Seccode = data.Symbol.ToUpper()
                        };

                        _abstractTerminal.AddSecurity(securities);
                    });

                    _bybitSocket.BufferSpotTickers.LinkTo(transferTickersToTerminal);
                }
                else if (_typeMarket == ConfigTermins.TypeCryptoAccountFutures)
                {
                    _bybitSocket.SubcribeToInstrumentsAsync(avalibleInstrumentsModel, _typeMarket);
                }
            });
        }

        public async Task GetBalancesAsync()
        {
            if (_typeMarket == ConfigTermins.TypeCryptoAccountSpot)
            {
                var balances = await _bybitRest.GetBalancesAccountAsync();

                var moneyPositions = balances.Select(x => new MoneyPosition()
                {
                    ClientCode = _secret,
                    Account = _key,
                    Asset = x.Asset.ToUpper(),
                    Currency = x.Asset.ToUpper(),
                    Balance = Convert.ToDouble(x.Available),
                    TotalBalance = Convert.ToDouble(x.Total),
                    Group = $"{NameExchange} {_typeMarket}",
                });

                foreach (var money in moneyPositions)
                {
                    _abstractTerminal.AddMoneyShare(_abstractTerminal.BaseTable.LimitMoneySharesList, money);
                }
            }
            else if (_typeMarket == ConfigTermins.TypeCryptoAccountFutures)
            {
                var balances = await _bybitRest.GetBalancesUsdFuturesAccountAsync();

                var moneyPositions = balances.Select(x => new MoneyPosition()
                {
                    ClientCode = _secret,
                    Account = _key,
                    Asset = x.Key.ToUpper(),
                    Currency = x.Key.ToUpper(),
                    Balance = (double)x.Value.AvailableBalance,
                    TotalBalance = (double)x.Value.TotalRealizedPnl,
                    Group = $"{NameExchange} {_typeMarket}",
                });

                foreach (var money in moneyPositions)
                {
                    _abstractTerminal.AddMoneyShare(_abstractTerminal.BaseTable.LimitMoneySharesList, money);
                }
            }
        }

        /// <summary>
        /// Получение общего баланса пользователя в USDT & USD по всем инструментам
        /// </summary>
        /// <returns></returns>
        public async Task GetBalanceByDollars() //TODO: реализовать
        {
            
        }

        public async Task GetHistoryOrdesAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModels)
        {
            var instrs = avalibleInstrumentsModels.Select(x => x.Symbol).ToList();

            if (_typeMarket == ConfigTermins.TypeCryptoAccountSpot)
            {
                var orders = await _bybitRest.GetOrdersAsync();
            }
            else if (_typeMarket == ConfigTermins.TypeCryptoAccountFutures)
            {
                Dictionary<string, Task<IEnumerable<BybitUsdPerpetualOrder>>> tasks = new Dictionary<string, Task<IEnumerable<BybitUsdPerpetualOrder>>>();
                instrs.ForEach(x => tasks.Add(x, _bybitRest.GetUsdFuturesOrdersAsync(x)));

                var orders = Task.WhenAll(tasks.Values).Result;
            }
        }

        public async Task<IEnumerable<Securities>> GetAllTickersAsync()
        {
            if (_typeMarket == ConfigTermins.TypeCryptoAccountSpot)
            {
                var tickers = await _bybitRest.GetTickersAsync();

                return tickers.Where(x => x.ShowStatus == true).Select(x => new Securities()
                {
                    Seccode = x.Name.ToUpper(),
                    ClassCode = NameExchange,
                    IsCrypto = true,
                    BaseActive = x.QuoteAsset.ToUpper(),
                    ShortName = x.BaseAsset.ToUpper(),
                    TradingStatus = "Торгуется",
                    Status = "Открыта",
                    Accuracy = (int)Math.Abs(Math.Log10(Convert.ToDouble(x?.PricePrecision))),
                    LotSize = Convert.ToDouble(x?.MinOrderQuantity),
                    MinStep = Convert.ToDouble(x?.PricePrecision),
                    PointCost = Convert.ToDouble(x?.MinOrderValue),
                    IsTrade = true,
                }).Where(ticker => !String.IsNullOrEmpty(ticker.Seccode) && !String.IsNullOrEmpty(ticker.BaseActive)).ToList();
            }
            else if (_typeMarket == ConfigTermins.TypeCryptoAccountFutures)
            {
                var tickers = await _bybitRest.GetUsdFuturesTickersAsync();

                return tickers.Where(x => x.Status == SymbolStatus.Trading).Select(x => new Securities()
                {
                    Seccode = x.Name.ToUpper(),
                    ClassCode = NameExchange,
                    IsCrypto = true,
                    BaseActive = x.QuoteCurrency.ToUpper(),
                    ShortName = x.BaseCurrency.ToUpper(),
                    TradingStatus = "Торгуется",
                    Status = "Открыта",
                    Accuracy = (int)Math.Abs(Math.Log10(Convert.ToDouble(x?.PricePrecision))),
                    LotSize = x.LotSizeFilter != null ? Convert.ToDouble(x.LotSizeFilter.MinQuantity) : 0,
                    MinStep = x.PriceFilter != null ? Convert.ToDouble(x?.PriceFilter.TickSize) : 0,
                    PointCost = x.LotSizeFilter != null ? Convert.ToDouble(x?.LotSizeFilter.QuantityStep) : 0,
                    IsTrade = true,
                }).Where(ticker => !String.IsNullOrEmpty(ticker.Seccode) && !String.IsNullOrEmpty(ticker.BaseActive)).ToList();
            }
            return new List<Securities>();
        }

        public async Task SubscribeToGlass(List<AvalibleInstrumentsModel> avalibleInstrumentsModels, int levels = 25)
        {
            if (!avalibleInstrumentsModels.Any())
                return;

            await _bybitSocket.SubcribeToGlasses(_typeMarket, avalibleInstrumentsModels, levels);

            if (_typeMarket == ConfigTermins.TypeCryptoAccountSpot)
            {
                var transferToGlass = new ActionBlock<BybitSpotOrderBookUpdate>(x =>
                {
                    var asks = x.Asks.Select(x => new GlassQuotation()
                    {
                        SellQty = Convert.ToDouble(x.Quantity),
                        Price = Convert.ToDouble(x.Price)
                    }).ToList();
                    var bids = x.Bids.Select(x => new GlassQuotation()
                    {
                        BuyQty = Convert.ToDouble(x.Quantity),
                        Price = Convert.ToDouble(x.Price)
                    }).ToList();

                    Glass glass = new Glass()
                    {
                        ClassCode = $"{NameExchange} {_typeMarket}",
                        Symbol = x.Symbol.ToUpper(),
                        Deep = levels,
                    };

                    glass.QuotationsBuy.AddRange(bids);
                    glass.QuotationsSell.AddRange(asks);

                    ((BybitConnector)_abstractTerminal).UpdateGlass(glass);
                });

                _bybitSocket.BufferSpotGlass.LinkTo(transferToGlass);
            }
            else if (_typeMarket == ConfigTermins.TypeCryptoAccountFutures)
            {
                var transferToGlass = new ActionBlock<IEnumerable<BybitOrderBookEntry>>(x =>
                {
                    List<Glass> glasses = new List<Glass>();

                    foreach (var item in x)
                    {
                        if (glasses.Select(x => x.Symbol == item.Symbol).Count() == 0)
                        {
                            Glass glass = new Glass()
                            {
                                ClassCode = $"{NameExchange} {_typeMarket}",
                                Symbol = item.Symbol.ToUpper(),
                                Deep = levels,
                            };

                            var asks = x.Where(x => x.Symbol == item.Symbol).ToList()
                            .Select(x => new GlassQuotation()
                            {
                                SellQty = Convert.ToDouble(x.Quantity),
                                Price = Convert.ToDouble(x.Price)
                            }).ToList();

                            var bids = x.Where(x => x.Symbol == item.Symbol).ToList()
                            .Select(x => new GlassQuotation()
                            {
                                BuyQty = Convert.ToDouble(x.Quantity),
                                Price = Convert.ToDouble(x.Price)
                            }).ToList();

                            glass.QuotationsBuy.AddRange(bids);
                            glass.QuotationsSell.AddRange(asks);

                            ((BybitConnector)_abstractTerminal).UpdateGlass(glass);
                        }
                        else
                            continue;
                    }
                });

                _bybitSocket.BufferUsdFuturesGlass.LinkTo(transferToGlass);
            }
        }

        public async Task PlaceOrderAsync(string symbol, OrderSide side, OrderType type, decimal quantity, decimal? price = null, 
            TimeInForce timeInForce = default, string clientOrderId = null, long? receiveWindow = null)
        {
            if (_typeMarket == ConfigTermins.TypeCryptoAccountSpot)
            {
                await _bybitRest.PlaceOrderAsync(symbol, side, type, quantity, price, timeInForce, clientOrderId, receiveWindow);

                var transferToTerminalOrders = new ActionBlock<BybitSpotOrderPlaced>(x =>
                {
                    var etsOrder = new Orders()
                    {
                        Status = ConfigTermins.Active,
                        Account = _key,
                        ClientCode = _secret,
                        Balance = Convert.ToDouble(x.Quantity),
                        Price = Convert.ToDouble(x.Price),
                        Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                        Number = x.Id.ToString(),
                        Symbol = x.Symbol is null ? string.Empty : x.Symbol,
                        ClassCode = $"Binance {_typeMarket}",
                        Quantity = Convert.ToDouble(x.Quantity),
                        Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                    };

                    _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, etsOrder);
                });

                _bybitRest.BufferPlaceSpotOrders.LinkTo(transferToTerminalOrders);
            }
            else if (_typeMarket == ConfigTermins.TypeCryptoAccountFutures)
            {
                await _bybitRest.PlaceUsdFuturesOrderAsync(symbol, side, type, quantity, timeInForce, price, clientOrderId, receiveWindow);

                var transferToTerminalOrders = new ActionBlock<BybitUsdPerpetualOrder>(x =>
                {
                    var etsOrder = new Orders()
                    {
                        Status = ConfigTermins.Active,
                        Account = _key,
                        ClientCode = _secret,
                        Balance = Convert.ToDouble(x.Quantity),
                        Price = Convert.ToDouble(x.Price),
                        Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                        Number = x.Id.ToString(),
                        Symbol = x.Symbol is null ? string.Empty : x.Symbol,
                        ClassCode = $"Binance {_typeMarket}",
                        Quantity = Convert.ToDouble(x.Quantity),
                        Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                    };

                    _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, etsOrder);
                });

                _bybitRest.BufferPlaceUsdFuturesOrders.LinkTo(transferToTerminalOrders);
            }
        }

        public async Task CancelOrderAsync(string symbol, long orderId)
        {
            if (_typeMarket == ConfigTermins.TypeCryptoAccountSpot)
            {
                await _bybitRest.CancelOrderAsync(orderId);

                var transferToTerminalOrders = new ActionBlock<BybitSpotOrderPlaced>(x =>
                {
                    var etsOrder = new Orders()
                    {
                        Status = ConfigTermins.Cancel,
                        Account = _key,
                        ClientCode = _secret,
                        Balance = Convert.ToDouble(x.Quantity),
                        Price = Convert.ToDouble(x.Price),
                        Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                        Number = x.Id.ToString(),
                        Symbol = x.Symbol is null ? string.Empty : x.Symbol,
                        ClassCode = $"Binance {_typeMarket}",
                        Quantity = Convert.ToDouble(x.Quantity),
                        Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                    };

                    _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, etsOrder);
                });

                _bybitRest.BufferCancelSpotOrders.LinkTo(transferToTerminalOrders);
            }
            else if (_typeMarket == ConfigTermins.TypeCryptoAccountFutures)
            {
                await _bybitRest.CancelUsdFuturesOrderAsync(symbol, orderId.ToString());

                var transferToTerminalOrders = new ActionBlock<BybitOrderId>(x =>
                {
                    var placedOrders = _bybitRest.BufferPlaceUsdFuturesOrders.ReceiveAllAsync().ToListAsync().Result;                   // TODO: сделал костыль, не уверен, что будет работать
                    BybitUsdPerpetualOrder? perpetualOrder = placedOrders.Find(x => x.Symbol == symbol && x.Id == orderId.ToString());

                    if (perpetualOrder != null)
                    {
                        var etsOrder = new Orders()
                        {
                            Status = ConfigTermins.Cancel,
                            Account = _key,
                            ClientCode = _secret,
                            Balance = Convert.ToDouble(perpetualOrder.Quantity),
                            Price = Convert.ToDouble(perpetualOrder.Price),
                            Id = perpetualOrder.ClientOrderId is null ? string.Empty : perpetualOrder.ClientOrderId,
                            Number = x.OrderId.ToString(),
                            Symbol = perpetualOrder.Symbol is null ? string.Empty : perpetualOrder.Symbol,
                            ClassCode = $"Binance {_typeMarket}",
                            Quantity = Convert.ToDouble(perpetualOrder.Quantity),
                            Operation = perpetualOrder.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                        };

                        _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, etsOrder);
                    }
                });

                _bybitRest.BufferCancelUsdFuturesOrders.LinkTo(transferToTerminalOrders);
            }
        }

        public async Task GetOrderBookAsync(string symbol, int limit = 100)
        {
            if (_typeMarket == ConfigTermins.TypeCryptoAccountSpot)
            {
                await _bybitRest.GetOrderBookAsync(symbol, limit);

                var transferToGlass = new ActionBlock<BybitSpotOrderBook>(x =>
                {
                    var asks = x.Asks.Select(x => new GlassQuotation()
                    {
                        SellQty = Convert.ToDouble(x.Quantity),
                        Price = Convert.ToDouble(x.Price)
                    }).ToList();

                    var bids = x.Bids.Select(x => new GlassQuotation()
                    {
                        BuyQty = Convert.ToDouble(x.Quantity),
                        Price = Convert.ToDouble(x.Price)
                    }).ToList();

                    Glass glass = new Glass()
                    {
                        ClassCode = $"{NameExchange} {_typeMarket}",
                        Symbol = symbol.ToUpper(),
                        Deep = limit,
                    };

                    glass.QuotationsBuy.AddRange(bids);
                    glass.QuotationsSell.AddRange(asks);

                    //((BinanceDima80LVL)_abstractTerminal).UpdateGlass(glass); TODO: need to realize
                });

                _bybitRest.BufferSpotOrderBook.LinkTo(transferToGlass);
            }
            else if (_typeMarket == ConfigTermins.TypeCryptoAccountFutures)
            {
                await _bybitRest.GetUsdFuturesOrderBookAsync(symbol);

                var transferToGlass = new ActionBlock<IEnumerable<BybitOrderBookEntry>>(x =>
                {
                    List<Glass> glasses = new List<Glass>();

                    foreach (var item in x)
                    {
                        if (glasses.Select(x => x.Symbol == item.Symbol).Count() == 0)
                        {
                            Glass glass = new Glass()
                            {
                                ClassCode = $"{NameExchange} {_typeMarket}",
                                Symbol = item.Symbol.ToUpper(),
                                Deep = limit,
                            };

                            var asks = x.Where(x => x.Symbol == item.Symbol).ToList()
                            .Select(x => new GlassQuotation()
                            {
                                SellQty = Convert.ToDouble(x.Quantity),
                                Price = Convert.ToDouble(x.Price)
                            }).ToList();

                            var bids = x.Where(x => x.Symbol == item.Symbol).ToList()
                            .Select(x => new GlassQuotation()
                            {
                                BuyQty = Convert.ToDouble(x.Quantity),
                                Price = Convert.ToDouble(x.Price)
                            }).ToList();

                            glass.QuotationsBuy.AddRange(bids);
                            glass.QuotationsSell.AddRange(asks);

                            //((BinanceDima80LVL)_abstractTerminal).UpdateGlass(glass); // TODO: to realize
                        }
                        else
                            continue;
                    }
                });
                _bybitRest.BufferUsdFuturesOrderBook.LinkTo(transferToGlass);
            }
        }

        public async Task<Candles> GetHistoryKlinesAsync(AvalibleInstrumentsModel avalibleInstruments, string timeFrame, DateTime dateStart, DateTime dateEnd)
        {
            if (_typeMarket == ConfigTermins.TypeCryptoAccountSpot)
            {
                List<BybitSpotKline> lst = new List<BybitSpotKline>();

                DateTime firstKlineTime = (await _bybitRest.GetKlinesAsync(avalibleInstruments.Symbol, ConvertToKline(CfgSourceEts.DownLoader1Month), dateStart)).First().OpenTime;

                if (dateStart < firstKlineTime)
                    dateStart = firstKlineTime;

                DateTime endTime = dateStart;

                while (dateStart < dateEnd)
                {
                    AddTimeToEndTime(timeFrame, endTime);

                    var currentKlines = await _bybitRest.GetKlinesAsync(avalibleInstruments.Symbol, ConvertToKline(timeFrame), dateStart, endTime);

                    if (currentKlines.Any())
                    {
                        lst.AddRange(currentKlines);
                        dateStart = AddTimeToDateStart(dateStart, timeFrame);
                    }
                }

                Candles candles = new Candles();

                foreach (var item in lst)
                {
                    candles.Candle.Add(new CandleModel()
                    {
                        Close = Convert.ToDouble(item.ClosePrice),
                        High = Convert.ToDouble(item.HighPrice),
                        Open = Convert.ToDouble(item.OpenPrice),
                        Low = Convert.ToDouble(item.LowPrice),
                        Volume = Convert.ToDouble(item.Volume),
                        TradeDateTime = item.OpenTime,
                    });
                }

                return candles;
            }
            else if (_typeMarket == ConfigTermins.TypeCryptoAccountFutures)
            {
                List<BybitKline> lst = new List<BybitKline>();

                DateTime firstKlineTime = (await _bybitRest.GetUsdFuturesKlinesAsync(avalibleInstruments.Symbol, ConvertToKline(CfgSourceEts.DownLoader1Month), dateStart)).First().OpenTime;

                if (dateStart < firstKlineTime)
                    dateStart = firstKlineTime;

                DateTime endTime = dateStart;

                while (dateStart < dateEnd)
                {
                    AddTimeToEndTime(timeFrame, endTime);

                    var currentKlines = await _bybitRest.GetUsdFuturesKlinesAsync(avalibleInstruments.Symbol, ConvertToKline(timeFrame), dateStart);

                    if (currentKlines.Any())
                    {
                        lst.AddRange(currentKlines);
                        dateStart = AddTimeToDateStart(dateStart, timeFrame);
                    }
                }

                Candles candles = new Candles();

                foreach (var item in lst)
                {
                    candles.Candle.Add(new CandleModel()
                    {
                        Close = Convert.ToDouble(item.ClosePrice),
                        High = Convert.ToDouble(item.HighPrice),
                        Open = Convert.ToDouble(item.OpenPrice),
                        Low = Convert.ToDouble(item.LowPrice),
                        Volume = Convert.ToDouble(item.Volume),
                        TradeDateTime = item.OpenTime,
                    });
                }

                return candles;
            }

            return new Candles();
        }

        public async Task SubscribeToTradesAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            await Task.Run(() =>
            {
                _bybitSocket.SubscribeToTradesAsync(avalibleInstrumentsModel, _typeMarket);

                var transferTrades = new ActionBlock<Trades>(x =>
                {
                    Tick tick = new Tick()
                    {
                        Volume = Convert.ToDouble(x.Quantity),
                        TradeDateTime = x.TradeTime,
                        Price = Convert.ToDouble(x.Price),
                        BuySell = x.IsSell ? CfgSourceEts.SellForAllTradeTable : CfgSourceEts.BuyForAllTradeTable,
                        Seccode = x.Symbol
                    };
                });

                _bybitSocket.BufferTrades.LinkTo(transferTrades);
            });
        }

        public async Task GetOpenOrdersAsync(string? symbol = null)
        {
            if (_typeMarket == ConfigTermins.TypeCryptoAccountSpot)
            {
                var openOrders = await _bybitRest.GetOpenOrdersAsync(symbol);
                if (openOrders.Data.Any())
                {
                    var orders = openOrders.Data.Select(x => new Orders()
                    {
                        Status = ConfigTermins.Active,
                        Account = _key,
                        ClientCode = _secret,
                        Balance = Convert.ToDouble(x.QuoteQuantity - x.QuantityFilled),
                        Price = Convert.ToDouble(x.Price),
                        Number = x.Id.ToString(),
                        Time = x.CreateTime,
                        Symbol = x.Symbol,
                        ClassCode = $"{NameExchange} {_typeMarket}",
                        Quantity = Convert.ToDouble(x.Quantity),
                        Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                    }).ToList();

                    foreach (var order in orders)
                    {
                        _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                    }
                }
            }
            else if (_typeMarket == ConfigTermins.TypeCryptoAccountFutures)
            {
                List<BybitUsdPerpetualOrder> openOrdersList = new List<BybitUsdPerpetualOrder>();

                if (symbol == null)
                {
                    var instrs = await _bybitRest.GetUsdFuturesTickersAsync();

                    if (instrs.Count() > 0)
                        instrs.ToList().ForEach(async x =>
                        {
                            var result = await _bybitRest.GetOpenOrdersRealTimeAsync(x.Name);

                            if (result.Success && result.Data.Count() > 0)
                                openOrdersList.AddRange(result.Data);
                        });
                }
                else
                {
                    var result = await _bybitRest.GetOpenOrdersRealTimeAsync(symbol);

                    if (result.Success)
                        openOrdersList = result.Data.ToList();
                }


                if (openOrdersList.Any())
                {
                    var orders = openOrdersList.Select(x => new Orders()
                    {
                        Status = ConfigTermins.Active,
                        Account = _key,
                        ClientCode = _secret,
                        Balance = Convert.ToDouble(x.Quantity - x.QuoteQuantityFilled),
                        Price = Convert.ToDouble(x.Price),
                        Number = x.Id.ToString(),
                        Time = x.LastTradeTime.GetValueOrDefault(),
                        Symbol = x.Symbol,
                        ClassCode = $"{NameExchange} {_typeMarket}",
                        Quantity = Convert.ToDouble(x.Quantity),
                        Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                    }).ToList();

                    foreach (var order in orders)
                    {
                        _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                    }
                }
            }
        }



        #region Helpers

        private KlineInterval ConvertToKline(string timeFrame)
        {
            if (timeFrame == CfgSourceEts.DownLoader1Minutes)
                return KlineInterval.OneMinute;
            else if (timeFrame == CfgSourceEts.DownLoader5Minutes)
                return KlineInterval.FiveMinutes;
            else if (timeFrame == CfgSourceEts.DownLoader15Minutes)
                return KlineInterval.FifteenMinutes;
            else if (timeFrame == CfgSourceEts.DownLoader1Hour)
                return KlineInterval.OneHour;
            else if (timeFrame == CfgSourceEts.DownLoader4Hour)
                return KlineInterval.FourHours;
            else if (timeFrame == CfgSourceEts.DownLoader1Day)
                return KlineInterval.OneDay;
            else if (timeFrame == CfgSourceEts.DownLoader1Week)
                return KlineInterval.OneWeek;
            else if (timeFrame == CfgSourceEts.DownLoader1Month)
                return KlineInterval.OneMonth;

            return default;
        }

        private DateTime AddTimeToDateStart(DateTime dateStart, string timeFrame)
        {
            if (timeFrame.Equals(CfgSourceEts.DownLoader1Minutes))
                return dateStart.AddMinutes(1);

            if (timeFrame.Equals(CfgSourceEts.DownLoader5Minutes))
                return dateStart.AddMinutes(5);

            if (timeFrame.Equals(CfgSourceEts.DownLoader15Minutes))
                return dateStart.AddMinutes(15);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Hour))
                return dateStart.AddHours(1);

            if (timeFrame.Equals(CfgSourceEts.DownLoader4Hour))
                return dateStart.AddHours(4);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Day))
                return dateStart.AddDays(1);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Week))
                return dateStart.AddDays(7);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Week))
                return dateStart.AddMonths(1);

            return new DateTime();
        }

        private DateTime AddTimeToEndTime(string timeFrame, DateTime endTime)
        {
            if (timeFrame.Equals(CfgSourceEts.DownLoader1Minutes))
                return endTime.AddMinutes(1000);

            if (timeFrame.Equals(CfgSourceEts.DownLoader5Minutes))
                return endTime.AddMinutes(1000 * 5);

            if (timeFrame.Equals(CfgSourceEts.DownLoader15Minutes))
                return endTime.AddMinutes(1000 * 15);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Hour))
                return endTime.AddHours(1000);

            if (timeFrame.Equals(CfgSourceEts.DownLoader4Hour))
                return endTime.AddHours(1000 * 4);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Day))
                return endTime.AddDays(1000);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Week))
                return endTime.AddDays(1000 * 7);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Week))
                return endTime.AddMonths(1000);

            return new DateTime();
        }

        #endregion
    }
}
