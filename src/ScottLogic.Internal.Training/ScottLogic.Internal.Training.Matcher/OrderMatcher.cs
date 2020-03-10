using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ScottLogic.Internal.Training.Matcher
{
    public class OrderMatcher
    {
        public Trade currentTrade { get; set; }
        public List<Order> ExistingOrders { get; set; } = new List<Order>();

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
            }
            else
            {
                // Get orders with opposite action, and account Number different than the current Order
                List<Order> oppositeOrders = ExistingOrders.FindAll(order => order.action != currentOrder.action && order.accountNumber != currentOrder.accountNumber);

                // No orders with opposite action
                if (oppositeOrders.Count == 0)
                {
                    ExistingOrders.Add(currentOrder);
                }
                else
                {
                    // Existing orders with opposite action, try a trade
                    if (currentOrder.action == "sell")
                    {
                        oppositeOrders = oppositeOrders.OrderByDescending(order => order.price).ThenBy(order => order.timeRank).ToList();

                        orderProcessed = SellTrade(currentOrder, oppositeOrders);
                    }
                    else
                    {
                        ;
                    }
                }
                
            }
            return orderProcessed;
        }

        private bool SellTrade(Order currentOrder, List<Order> oppositeOrders)
        {

            bool orderProcessed = false;
            // 1. Sell entire quantity in one transaction
            // 2. Sell entire quantity in more than one transaction
            // 3. Order has quantity unsold

            // Find all possible matches
            oppositeOrders = oppositeOrders.Where(order => order.price >= currentOrder.price).ToList();
            if (oppositeOrders.Count == 0)
            {
                ExistingOrders.Add(currentOrder);
            }
            else
            {
                // Create a Trade
                currentTrade = new Trade(currentOrder.accountNumber, 0, currentOrder.price, currentOrder.action);
                for (int i = 0; i < oppositeOrders.Count; i++)
                {
                    orderProcessed = true;
                    if (currentOrder.quantity <= oppositeOrders[i].quantity)
                    {
                        int index = oppositeOrders.IndexOf(oppositeOrders.Single(order => order.timeRank == oppositeOrders[i].timeRank));
                        if (currentOrder.quantity != oppositeOrders[i].quantity)
                        {
                            // Update the other order
                            ExistingOrders[index].quantity -= currentOrder.quantity;
                        }
                        else
                        {
                            // Remove the other order
                            ExistingOrders = ExistingOrders.Where(order => order.timeRank != oppositeOrders[i].timeRank).ToList();
                        }

                        currentTrade.quantity += currentOrder.quantity;
                        currentOrder.quantity = 0;
                        break;
                    }
                    // We sell more than the current match
                    else
                    {
                        // partial match - current order sells more than the demand
                        // Sell the amount required, delete that order, update current order
                        int tradeQuantity = oppositeOrders[i].quantity;
                        currentOrder.quantity -= tradeQuantity;
                        // Remove the other order
                        ExistingOrders = ExistingOrders.Where(order => order.timeRank != oppositeOrders[i].timeRank).ToList();
                        currentTrade.quantity += tradeQuantity;
                    }
                }
                // Current order was not fully processed 
                if (currentOrder.quantity > 0)
                {
                    ExistingOrders.Add(currentOrder);
                }
            }
            return orderProcessed;
        }

        bool BuyTrade(Order currentOrder, List<Order> oppositeOrders)
        {
            return false;
        }
    }
}
