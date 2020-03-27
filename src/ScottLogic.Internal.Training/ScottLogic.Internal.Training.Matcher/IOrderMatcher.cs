using System.Collections.Generic;

namespace ScottLogic.Internal.Training.Matcher
{
    public interface IOrderMatcher
    {
        List<Order> ExistingOrders { get; set; }
        List<Trade> ExistingTrades { get; set; }
        bool ProcessOrder(Order currentOrder);
    }
}
