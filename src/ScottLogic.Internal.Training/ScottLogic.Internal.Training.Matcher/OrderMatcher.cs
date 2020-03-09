using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ScottLogic.Internal.Training.Matcher
{
    public class OrderMatcher
    {
        public Trade currentTrade { get; set; }
        public List<Order> ExistingOrders { get; } = new List<Order>();

        public OrderMatcher()
        {
            currentTrade = null;
        }

        public bool ProcessOrder(Order currentOrder)
        {
            bool orderProcessed = false;
            // No existing order to match against
            if (ExistingOrders.Count == 0)
            {
                ExistingOrders.Add(currentOrder);
                return orderProcessed;
            }

            List<Order> oppositeOrders = ExistingOrders.FindAll(order => order.action != currentOrder.action && order.accountNumber != currentOrder.accountNumber);

            // No orders with opposite action
            if (oppositeOrders.Count == 0)
            {
                ExistingOrders.Add(currentOrder);
                return orderProcessed;
            }
            // Existing orders with opposite action, try a trade
            if (currentOrder.action == "sell")
            {
                oppositeOrders.Sort(
                    delegate (Order order1, Order order2)
                    {
                        if (order1.price == order2.price)
                        {
                            return order1.timeRank.CompareTo(order2.timeRank);
                        }
                        else
                        {
                            return order2.price.CompareTo(order1.price);
                        }
                    }
                    );
                orderProcessed = SellTrade(currentOrder, oppositeOrders);
            }
            else
            {
                ;
            }

            bool SellTrade(Order currentOrder, List<Order> oppositeOrders)
            {

                bool orderProcessed = false;
                // 1. Sell entire quantity in one transaction
                // 2. Sell entire quantity in more than one transaction
                // 3. Order has quantity unsold

                // Find all possible matches
                oppositeOrders = oppositeOrders.FindAll(order => order.price >= currentOrder.price);
                if (oppositeOrders.Count == 0)
                {
                    ExistingOrders.Add(currentOrder);
                    return orderProcessed;
                }
                else
                {
                    // Create a Trade
                    currentTrade = new Trade();
                }
                return orderProcessed;
            }

            return orderProcessed;
        }
    }
}
