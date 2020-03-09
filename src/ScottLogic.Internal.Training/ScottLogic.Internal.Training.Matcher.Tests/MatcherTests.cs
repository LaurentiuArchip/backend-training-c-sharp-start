using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

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
            Trade currentTrade = new Trade();
            Assert.IsType<Trade>(currentTrade);
        }
    }
}
