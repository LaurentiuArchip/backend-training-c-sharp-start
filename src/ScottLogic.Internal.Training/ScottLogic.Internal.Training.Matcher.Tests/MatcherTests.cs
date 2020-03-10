using System.Collections.Generic;
using Xunit;


namespace ScottLogic.Internal.Training.Matcher.Tests
{
    public class MatcherTests
    {
        [Fact]
        public void MatcherExists()
        {
            var currentMatcher = new OrderMatcher();
            Assert.IsType<OrderMatcher>(currentMatcher);
        }
    }

    public class ProcessOrder
    {
        [Fact]
        public void AddBuyOrder_EmptyMatcher_OneOrderAdded()
        {
            var currentOrder = new Order(1001, 45, 55, "buy", 13);
            var currentMatcher = new OrderMatcher();
            currentMatcher.ProcessOrder(currentOrder);
            Assert.Same(currentOrder, currentMatcher.ExistingOrders[0]);
        }

        [Fact]
        public void AddSellOrder_EmptyMatcher_OneOrderAdded()
        {
            var currentOrder = new Order(1001, 45, 55, "sell", 13);
            var currentMatcher = new OrderMatcher();
            currentMatcher.ProcessOrder(currentOrder);
            Assert.Same(currentOrder, currentMatcher.ExistingOrders[0]);
        }

        [Fact]
        public void AddSellOrder_OneExistingOrder_MatcherWithTwoOrders()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 45, 55, "sell", 14);
            var currentOrder2 = new Order(1002, 46, 56, "sell", 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var orders = new List<Order>() { currentOrder1, currentOrder2 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddBuyOrder_OneExistingOrder_MatcherWithTwoOrders()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 45, 55, "buy", 14);
            var currentOrder2 = new Order(1002, 46, 56, "buy", 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var orders = new List<Order>() { currentOrder1, currentOrder2 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddSellOrder_PerfectMatch_ExistingOrdersEmpty()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 45, 55, "buy", 14);
            var currentOrder2 = new Order(1002, 45, 55, "sell", 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var orders = new List<Order>() {};
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddBuyOrder_PerfectMatch_ExistingOrdersEmpty()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 45, 55, "sell", 14);
            var currentOrder2 = new Order(1002, 45, 55, "buy", 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var orders = new List<Order>() { };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddSellOrder_SellLess_PartialMatch()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 55, 55, "buy", 13);
            var currentOrder2 = new Order(1002, 45, 55, "sell", 14);

            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);

            var currentOrder4 = new Order(1001, 10, 55, "buy", 13);

            var orders = new List<Order>() { currentOrder4 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddBuyOrder_BuyLess_PartialMatch()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 55, 55, "sell", 13);
            var currentOrder2 = new Order(1002, 45, 55, "buy", 14);

            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);

            var currentOrder4 = new Order(1001, 10, 55, "sell", 13);

            var orders = new List<Order>() { currentOrder4 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddSellOrder_SellMore_PartialMatch()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 55, 55, "buy", 13);
            var currentOrder2 = new Order(1002, 65, 55, "sell", 14);

            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);

            var currentOrder4 = new Order(1002, 10, 55, "sell", 14);

            var orders = new List<Order>() { currentOrder4 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddBuyOrder_BuyMore_PartialMatch()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 55, 55, "sell", 13);
            var currentOrder2 = new Order(1002, 65, 55, "buy", 14);

            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);

            var currentOrder4 = new Order(1002, 10, 55, "buy", 14);

            var orders = new List<Order>() { currentOrder4 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }
    }

    public class ReturnTrade
    {
        [Fact]
        public void TradeExists()
        {
            var currentTrade = new Trade(0, 0, 0, "buy");
            Assert.IsType<Trade>(currentTrade);
        }

        [Fact]
        public void GetNullTrade()
        {
            var currentMatcher = new OrderMatcher();
            Assert.Null(currentMatcher.CurrentTrade);
        }

        [Fact]
        public void AddSellOrder_EmptyMatcher_NullTrade()
        {
            var currentOrder = new Order(1001, 45, 55, "sell", 13);
            var currentMatcher = new OrderMatcher();
            currentMatcher.ProcessOrder(currentOrder);
            var currentTrade = currentMatcher.CurrentTrade;
            Assert.Null(currentTrade);
        }

        [Fact]
        public void AddBuyOrder_EmptyMatcher_NullTrade()
        {
            var currentOrder = new Order(1001, 45, 55, "buy", 13);
            var currentMatcher = new OrderMatcher();
            currentMatcher.ProcessOrder(currentOrder);
            var currentTrade = currentMatcher.CurrentTrade;
            Assert.Null(currentTrade);
        }

        [Fact]
        public void AddSellOrder_PerfectMatch_NewTrade()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 45, 55, "buy", 14);
            var currentOrder2 = new Order(1002, 45, 55, "sell", 15);
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
            var currentOrder1 = new Order(1001, 45, 55, "sell", 14);
            var currentOrder2 = new Order(1002, 45, 55, "buy", 15);
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
            var currentOrder1 = new Order(1001, 45, 55, "buy", 14);
            var currentOrder2 = new Order(1002, 55, 55, "sell", 15);
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
            var currentOrder1 = new Order(1001, 45, 55, "sell", 14);
            var currentOrder2 = new Order(1002, 55, 55, "buy", 15);
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
            var currentOrder1 = new Order(1001, 65, 55, "buy", 14);
            var currentOrder2 = new Order(1002, 55, 55, "sell", 15);
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
            var currentOrder1 = new Order(1001, 65, 55, "sell", 14);
            var currentOrder2 = new Order(1002, 55, 55, "buy", 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var returnedTrade = currentMatcher.CurrentTrade;
            var currentTrade = new Trade(currentOrder2.AccountNumber, 55, currentOrder2.Price, currentOrder2.Action);
            Assert.Equal(currentTrade, returnedTrade);
        }
    }
}
