using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ScottLogic.Internal.Training.Matcher.Tests
{
    public class TestMultipleOrders
    {
        [Fact]
        public void SellTradeByTimeRank()
        {
            OrderMatcher currentMatcher = new OrderMatcher();
            Order currentOrder1 = new Order(1001, 45, 55, "buy", 13);
            Order currentOrder2 = new Order(1002, 46, 56, "buy", 14);
            Order currentOrder3 = new Order(1003, 46, 56, "buy", 4);
            Order currentOrder4 = new Order(1004, 46, 56, "buy", 1);
            Order currentOrder5 = new Order(1005, 46, 56, "sell", 15);
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
