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
        readonly Order _currentOrder = new Order(1001, 45, 55, "buy", 13);
        readonly OrderMatcher _currentMatcher = new OrderMatcher();

        [Fact]
        public void MatcherExists()
        {
            Assert.IsType<OrderMatcher>(_currentMatcher);
        }

        [Fact]
        public void TradeExists()
        {
            Trade currentTrade = new Trade();
            Assert.IsType<Trade>(currentTrade);
        }

        [Fact]
        public void MatcherTakesOrder()
        {
            _currentMatcher.ProcessOrder(_currentOrder);
            Assert.Same(_currentOrder, _currentMatcher.ExistingOrders[0]);
        }

        [Fact]
        public void AddSecondOrder()
        {
            OrderMatcher _currentMatcher2 = new OrderMatcher();
            Order _currentOrder1 = new Order(1001, 45, 55, "buy", 14);
            Order currentOrder2 = new Order(1002, 46, 56, "buy", 15);
            _currentMatcher2.ProcessOrder(_currentOrder1);
            _currentMatcher2.ProcessOrder(currentOrder2);
            var orders = new List<Order>() {_currentOrder1, currentOrder2};
            Assert.Equal<List<Order>>(orders, _currentMatcher2.ExistingOrders);
        }

        [Fact]
        public void GetNullTrade()
        {
            Assert.Null(_currentMatcher.currentTrade);
        }

        [Fact]
        public void sellTrade()
        {
            OrderMatcher _currentMatcher = new OrderMatcher();
            Order _currentOrder1 = new Order(1001, 45, 55, "buy", 13);
            Order currentOrder2 = new Order(1002, 46, 56, "buy", 14);
            Order currentOrder3 = new Order(1003, 46, 56, "buy", 4);
            Order currentOrder4 = new Order(1004, 46, 56, "buy", 1);
            Order currentOrder5 = new Order(1005, 46, 56, "sell", 15);
            _currentMatcher.ProcessOrder(_currentOrder1);
            _currentMatcher.ProcessOrder(currentOrder2);
            _currentMatcher.ProcessOrder(currentOrder3);
            _currentMatcher.ProcessOrder(currentOrder4);
            _currentMatcher.ProcessOrder(currentOrder5);
            var orders = new List<Order>() { _currentOrder1};
            Assert.Equal<List<Order>>(orders, _currentMatcher.ExistingOrders);
        }
    }
}
