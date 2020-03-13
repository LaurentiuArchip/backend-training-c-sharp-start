using System.Collections.Generic;
using Xunit;


namespace ScottLogic.Internal.Training.Matcher.Tests
{
    public class ProcessOrder
    {
        [Fact]
        public void AddBuyOrder_EmptyMatcher_OneOrderAdded()
        {
            var currentOrder = new Order(1001, 45, 55, OrderType.Buy, 13);
            var currentMatcher = new OrderMatcher();
            currentMatcher.ProcessOrder(currentOrder);
            Assert.Same(currentOrder, currentMatcher.ExistingOrders[0]);
        }

        [Fact]
        public void AddSellOrder_EmptyMatcher_OneOrderAdded()
        {
            var currentOrder = new Order(1001, 45, 55, OrderType.Sell, 13);
            var currentMatcher = new OrderMatcher();
            currentMatcher.ProcessOrder(currentOrder);
            Assert.Same(currentOrder, currentMatcher.ExistingOrders[0]);
        }

        [Fact]
        public void AddSellOrder_OneExistingOrder_MatcherWithTwoOrders()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 45, 55, OrderType.Sell, 14);
            var currentOrder2 = new Order(1002, 46, 56, OrderType.Sell, 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var orders = new List<Order>() { currentOrder1, currentOrder2 };
            Assert.Equal(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddBuyOrder_OneExistingOrder_MatcherWithTwoOrders()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 45, 55, OrderType.Buy, 14);
            var currentOrder2 = new Order(1002, 46, 56, OrderType.Buy, 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var orders = new List<Order>() { currentOrder1, currentOrder2 };
            Assert.Equal(orders, currentMatcher.ExistingOrders);
        }
    }

    public class SellTrade
    {
        [Fact]
        public void AddSellOrder_PerfectMatch_ExistingOrdersEmpty()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 45, 55, OrderType.Buy, 14);
            var currentOrder2 = new Order(1002, 45, 55, OrderType.Sell, 15);
            currentMatcher.ExistingOrders.Add(currentOrder1);
            var oppositeOrders = new List<Order>(){currentOrder1};
            currentMatcher.SellTrade(currentOrder2, oppositeOrders);
            Assert.Empty(currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddSellOrder_SellLess_PartialMatch()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 55, 55, OrderType.Buy, 13);
            var currentOrder2 = new Order(1002, 45, 55, OrderType.Sell, 14);

            currentMatcher.ExistingOrders.Add(currentOrder1);
            var oppositeOrders = new List<Order>() { currentOrder1 };
            currentMatcher.SellTrade(currentOrder2, oppositeOrders);
            var currentOrder4 = new Order(1001, 10, 55, OrderType.Buy, 13);
            
            var orders = new List<Order>() { currentOrder4 };
            Assert.Equal(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddSellOrder_SellMore_PartialMatch()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 55, 55, OrderType.Buy, 13);
            var currentOrder2 = new Order(1002, 65, 55, OrderType.Sell, 14);

            currentMatcher.ExistingOrders.Add(currentOrder1);
            var oppositeOrders = new List<Order>() { currentOrder1 };
            currentMatcher.SellTrade(currentOrder2, oppositeOrders);
            var currentOrder4 = new Order(1002, 10, 55, OrderType.Sell, 14);

            var orders = new List<Order>() { currentOrder4 };
            Assert.Equal(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void SellOrder_ExistingOrdersByTimeRank_OldestOrderProcessed()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 45, 55, OrderType.Buy, 13);
            var currentOrder2 = new Order(1002, 46, 56, OrderType.Buy, 14);
            var currentOrder3 = new Order(1003, 46, 56, OrderType.Buy, 4);
            var currentOrder4 = new Order(1004, 46, 56, OrderType.Buy, 1);
            var currentOrder5 = new Order(1005, 46, 56, OrderType.Sell, 15);
            currentMatcher.ExistingOrders.Add(currentOrder1);
            currentMatcher.ExistingOrders.Add(currentOrder2);
            currentMatcher.ExistingOrders.Add(currentOrder3);
            currentMatcher.ExistingOrders.Add(currentOrder4);
            var oppositeOrders = new List<Order>() { currentOrder4, currentOrder3, currentOrder1, currentOrder2 };
            currentMatcher.SellTrade(currentOrder5, oppositeOrders);
            var orders = new List<Order>() { currentOrder1, currentOrder2, currentOrder3 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }
    }

    public class BuyTrade
    {
        [Fact]
        public void AddBuyOrder_PerfectMatch_ExistingOrdersEmpty()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 45, 55, OrderType.Sell, 14);
            var currentOrder2 = new Order(1002, 45, 55, OrderType.Buy, 15);
            currentMatcher.ExistingOrders.Add(currentOrder1);
            var oppositeOrders = new List<Order>() { currentOrder1 };
            currentMatcher.BuyTrade(currentOrder2, oppositeOrders);
            Assert.Empty(currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddBuyOrder_BuyLess_PartialMatch()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 55, 55, OrderType.Sell, 13);
            var currentOrder2 = new Order(1002, 45, 55, OrderType.Buy, 14);

            currentMatcher.ExistingOrders.Add(currentOrder1);
            var oppositeOrders = new List<Order>() { currentOrder1 };
            currentMatcher.BuyTrade(currentOrder2, oppositeOrders);
            var currentOrder4 = new Order(1001, 10, 55, OrderType.Sell, 13);

            var orders = new List<Order>() { currentOrder4 };
            Assert.Equal(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddBuyOrder_BuyMore_PartialMatch()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 55, 55, OrderType.Sell, 13);
            var currentOrder2 = new Order(1002, 65, 55, OrderType.Buy, 14);

            currentMatcher.ExistingOrders.Add(currentOrder1);
            var oppositeOrders = new List<Order>() { currentOrder1 };
            currentMatcher.BuyTrade(currentOrder2, oppositeOrders);
            var currentOrder4 = new Order(1002, 10, 55, OrderType.Buy, 14);

            var orders = new List<Order>() { currentOrder4 };
            Assert.Equal(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void BuyOrder_ExistingOrdersByTimeRank_OldestOrderProcessed()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 46, 56, OrderType.Sell, 13);
            var currentOrder2 = new Order(1002, 46, 56, OrderType.Sell, 14);
            var currentOrder3 = new Order(1003, 46, 56, OrderType.Sell, 4);
            var currentOrder4 = new Order(1004, 46, 56, OrderType.Sell, 1);
            var currentOrder5 = new Order(1005, 46, 56, OrderType.Buy, 15);
            currentMatcher.ExistingOrders.Add(currentOrder1);
            currentMatcher.ExistingOrders.Add(currentOrder2);
            currentMatcher.ExistingOrders.Add(currentOrder3);
            currentMatcher.ExistingOrders.Add(currentOrder4);
            var oppositeOrders = new List<Order>() { currentOrder4, currentOrder3, currentOrder1, currentOrder2 };
            currentMatcher.BuyTrade(currentOrder5, oppositeOrders);
            var orders = new List<Order>() { currentOrder1, currentOrder2, currentOrder3 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }
    }
    
    public class ReturnCurrentTrade
    {
        [Fact]
        public void EmptyMatcher_NullTrade()
        {
            var currentMatcher = new OrderMatcher();
            Assert.Null(currentMatcher.CurrentTrade);
        }

        [Fact]
        public void AddSellOrder_EmptyMatcher_NullTrade()
        {
            var currentOrder = new Order(1001, 45, 55, OrderType.Sell, 13);
            var currentMatcher = new OrderMatcher();
            currentMatcher.ProcessOrder(currentOrder);
            var currentTrade = currentMatcher.CurrentTrade;
            Assert.Null(currentTrade);
        }

        [Fact]
        public void AddBuyOrder_EmptyMatcher_NullTrade()
        {
            var currentOrder = new Order(1001, 45, 55, OrderType.Buy, 13);
            var currentMatcher = new OrderMatcher();
            currentMatcher.ProcessOrder(currentOrder);
            var currentTrade = currentMatcher.CurrentTrade;
            Assert.Null(currentTrade);
        }

        [Fact]
        public void AddSellOrder_PerfectMatch_NewTrade()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 45, 55, OrderType.Buy, 14);
            var currentOrder2 = new Order(1002, 45, 55, OrderType.Sell, 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var returnedTrade = currentMatcher.CurrentTrade;
            var currentTrade = new Trade(currentOrder2.AccountNumber, 45, currentOrder2.Price, currentOrder2.Action);
            Assert.Equal(currentTrade, returnedTrade);
        }

        [Fact]
        public void AddBuyOrder_PerfectMatch_NewTrade()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 45, 55, OrderType.Sell, 14);
            var currentOrder2 = new Order(1002, 45, 55, OrderType.Buy, 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var returnedTrade = currentMatcher.CurrentTrade;
            var currentTrade = new Trade(currentOrder2.AccountNumber, 45, currentOrder2.Price, currentOrder2.Action);
            Assert.Equal(currentTrade, returnedTrade);
        }

        [Fact]
        public void AddSellOrder_SellMore_PartialMatch_NewTrade()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 45, 55, OrderType.Buy, 14);
            var currentOrder2 = new Order(1002, 55, 55, OrderType.Sell, 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var returnedTrade = currentMatcher.CurrentTrade;
            var currentTrade = new Trade(currentOrder2.AccountNumber, 45, currentOrder2.Price, currentOrder2.Action);
            Assert.Equal(currentTrade, returnedTrade);
        }

        [Fact]
        public void AddBuyOrder_SellMore_PartialMatch_NewTrade()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 45, 55, OrderType.Sell, 14);
            var currentOrder2 = new Order(1002, 55, 55, OrderType.Buy, 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var returnedTrade = currentMatcher.CurrentTrade;
            var currentTrade = new Trade(currentOrder2.AccountNumber, 45, currentOrder2.Price, currentOrder2.Action);
            Assert.Equal(currentTrade, returnedTrade);
        }

        [Fact]
        public void AddSellOrder_SellLess_PartialMatch_NewTrade()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 65, 55, OrderType.Buy, 14);
            var currentOrder2 = new Order(1002, 55, 55, OrderType.Sell, 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var returnedTrade = currentMatcher.CurrentTrade;
            var currentTrade = new Trade(currentOrder2.AccountNumber, 55, currentOrder2.Price, currentOrder2.Action);
            Assert.Equal(currentTrade, returnedTrade);
        }

        [Fact]
        public void AddBuyOrder_BuyLess_PartialMatch_NewTrade()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 65, 55, OrderType.Sell, 14);
            var currentOrder2 = new Order(1002, 55, 55, OrderType.Buy, 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var returnedTrade = currentMatcher.CurrentTrade;
            var currentTrade = new Trade(currentOrder2.AccountNumber, 55, currentOrder2.Price, currentOrder2.Action);
            Assert.Equal(currentTrade, returnedTrade);
        }
    }

    public class ReturnExistingTrades
    {
        [Fact]
        public void NoExistingTrades()
        {
            var currentMatcher = new OrderMatcher();
            
            Assert.Empty(currentMatcher.ExistingTrades);
        }

        [Fact]
        public void OneExistingTrade()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 65, 55, OrderType.Sell, 14);
            var currentOrder2 = new Order(1002, 55, 55, OrderType.Buy, 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);

            var trades = new List<Trade>{ currentMatcher.CurrentTrade };

            Assert.Equal(trades, currentMatcher.ExistingTrades);
        }

        [Fact]
        public void TwoExistingTrades()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 65, 55, OrderType.Sell, 14);
            var currentOrder2 = new Order(1002, 65, 55, OrderType.Buy, 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);

            var currentOrder3 = new Order(1001, 75, 55, OrderType.Sell, 16);
            var currentOrder4 = new Order(1002, 75, 55, OrderType.Buy, 17);

            currentMatcher.ProcessOrder(currentOrder3);
            currentMatcher.ProcessOrder(currentOrder4);

            var currentTrade1 = new Trade(currentOrder2.AccountNumber, 65, currentOrder2.Price, currentOrder2.Action);
            var currentTrade2 = new Trade(currentOrder4.AccountNumber, 75, currentOrder4.Price, currentOrder4.Action);
            var trades = new List<Trade>() { currentTrade1, currentTrade2 };

            Assert.Equal(trades, currentMatcher.ExistingTrades);
        }
    }
}
