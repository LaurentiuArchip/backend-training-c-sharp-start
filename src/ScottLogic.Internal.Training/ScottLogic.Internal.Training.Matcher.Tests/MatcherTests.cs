using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;
using Xunit.Abstractions;


namespace ScottLogic.Internal.Training.Matcher.Tests
{
    public class MatcherTests
    {
        [Fact]
        public void MatcherExists()
        {
            OrderMatcher currentMatcher = new OrderMatcher();
            Assert.IsType<OrderMatcher>(currentMatcher);
        }

        [Fact]
        public void TradeExists()
        {
            Trade currentTrade = new Trade(0,0,0,"buy");
            Assert.IsType<Trade>(currentTrade);
        }
        
        [Fact]
        public void GetNullTrade()
        {
            OrderMatcher currentMatcher = new OrderMatcher();
            Assert.Null(currentMatcher.CurrentTrade);
        }
    }

    public class ProcessOrder
    {
        [Fact]
        public void AddBuyOrder_EmptyMatcher_OneOrderAdded()
        {
            Order currentOrder = new Order(1001, 45, 55, "buy", 13);
            OrderMatcher currentMatcher = new OrderMatcher();
            currentMatcher.ProcessOrder(currentOrder);
            Assert.Same(currentOrder, currentMatcher.ExistingOrders[0]);
        }

        [Fact]
        public void AddSellOrder_EmptyMatcher_OneOrderAdded()
        {
            Order currentOrder = new Order(1001, 45, 55, "sell", 13);
            OrderMatcher currentMatcher = new OrderMatcher();
            currentMatcher.ProcessOrder(currentOrder);
            Assert.Same(currentOrder, currentMatcher.ExistingOrders[0]);
        }

        [Fact]
        public void AddSellOrder_OneExistingOrder_MatcherWithTwoOrders()
        {
            OrderMatcher currentMatcher = new OrderMatcher();
            Order currentOrder1 = new Order(1001, 45, 55, "sell", 14);
            Order currentOrder2 = new Order(1002, 46, 56, "sell", 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var orders = new List<Order>() { currentOrder1, currentOrder2 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddBuyOrder_OneExistingOrder_MatcherWithTwoOrders()
        {
            OrderMatcher currentMatcher = new OrderMatcher();
            Order currentOrder1 = new Order(1001, 45, 55, "buy", 14);
            Order currentOrder2 = new Order(1002, 46, 56, "buy", 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var orders = new List<Order>() { currentOrder1, currentOrder2 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddSellOrder_PerfectMatch_ExistingOrdersEmpty()
        {
            OrderMatcher currentMatcher = new OrderMatcher();
            Order currentOrder1 = new Order(1001, 45, 55, "buy", 14);
            Order currentOrder2 = new Order(1002, 45, 55, "sell", 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var orders = new List<Order>() {};
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddBuyOrder_PerfectMatch_ExistingOrdersEmpty()
        {
            OrderMatcher currentMatcher = new OrderMatcher();
            Order currentOrder1 = new Order(1001, 45, 55, "sell", 14);
            Order currentOrder2 = new Order(1002, 45, 55, "buy", 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            var orders = new List<Order>() { };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddSellOrder_SellLess_PartialMatch()
        {
            OrderMatcher currentMatcher = new OrderMatcher();
            Order currentOrder1 = new Order(1001, 55, 55, "buy", 13);
            Order currentOrder2 = new Order(1002, 45, 55, "sell", 14);

            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);

            Order currentOrder4 = new Order(1001, 10, 55, "buy", 13);

            var orders = new List<Order>() { currentOrder4 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddBuyOrder_BuyLess_PartialMatch()
        {
            OrderMatcher currentMatcher = new OrderMatcher();
            Order currentOrder1 = new Order(1001, 55, 55, "sell", 13);
            Order currentOrder2 = new Order(1002, 45, 55, "buy", 14);

            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);

            Order currentOrder4 = new Order(1001, 10, 55, "sell", 13);

            var orders = new List<Order>() { currentOrder4 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddSellOrder_SellMore_PartialMatch()
        {
            OrderMatcher currentMatcher = new OrderMatcher();
            Order currentOrder1 = new Order(1001, 55, 55, "buy", 13);
            Order currentOrder2 = new Order(1002, 65, 55, "sell", 14);

            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);

            Order currentOrder4 = new Order(1002, 10, 55, "sell", 14);

            var orders = new List<Order>() { currentOrder4 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void AddBuyOrder_BuyMore_PartialMatch()
        {
            OrderMatcher currentMatcher = new OrderMatcher();
            Order currentOrder1 = new Order(1001, 55, 55, "sell", 13);
            Order currentOrder2 = new Order(1002, 65, 55, "buy", 14);

            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);

            Order currentOrder4 = new Order(1002, 10, 55, "buy", 14);

            var orders = new List<Order>() { currentOrder4 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }
    }
}
