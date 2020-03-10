using System.Collections.Generic;
using Xunit;

namespace ScottLogic.Internal.Training.Matcher.Tests
{
    public class TestMultipleOrders
    {
        [Fact]
        public void SellOrder_ExistingOrdersByTimeRank_OldestOrderProcessed()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 45, 55, "buy", 13);
            var currentOrder2 = new Order(1002, 46, 56, "buy", 14);
            var currentOrder3 = new Order(1003, 46, 56, "buy", 4);
            var currentOrder4 = new Order(1004, 46, 56, "buy", 1);
            var currentOrder5 = new Order(1005, 46, 56, "sell", 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            currentMatcher.ProcessOrder(currentOrder3);
            currentMatcher.ProcessOrder(currentOrder4);
            currentMatcher.ProcessOrder(currentOrder5);
            var orders = new List<Order>() { currentOrder1, currentOrder2, currentOrder3 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }

        [Fact]
        public void BuyOrder_ExistingOrdersByTimeRank_OldestOrderProcessed()
        {
            var currentMatcher = new OrderMatcher();
            var currentOrder1 = new Order(1001, 46, 56, "sell", 13);
            var currentOrder2 = new Order(1002, 46, 56, "sell", 14);
            var currentOrder3 = new Order(1003, 46, 56, "sell", 4);
            var currentOrder4 = new Order(1004, 46, 56, "sell", 1);
            var currentOrder5 = new Order(1005, 46, 56, "buy", 15);
            currentMatcher.ProcessOrder(currentOrder1);
            currentMatcher.ProcessOrder(currentOrder2);
            currentMatcher.ProcessOrder(currentOrder3);
            currentMatcher.ProcessOrder(currentOrder4);
            currentMatcher.ProcessOrder(currentOrder5);
            var orders = new List<Order>() { currentOrder1, currentOrder2, currentOrder3 };
            Assert.Equal<List<Order>>(orders, currentMatcher.ExistingOrders);
        }
    }
}
