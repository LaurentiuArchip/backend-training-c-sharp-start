using System.Collections.Generic;
using System.Linq;

namespace ScottLogic.Internal.Training.Matcher
{
    public class OrderMatcher : IOrderMatcher
    {
        public Trade CurrentTrade { get; set; }
        public List<Order> ExistingOrders { get; set; } = new List<Order>();

        public bool ProcessOrder(Order currentOrder)
        {
            var orderProcessed = false;
            // No existing order to match against
            if (!ExistingOrders.Any())
            {
                ExistingOrders.Add(currentOrder);
            }
            else
            {
                // Get orders with opposite action, and account Number different than the current Order
                var oppositeOrders = ExistingOrders
                    .Where(order => order.Action != currentOrder.Action && order.AccountNumber != currentOrder.AccountNumber)
                    .ToList();

                // No orders with opposite action
                if (!oppositeOrders.Any())
                {
                    ExistingOrders.Add(currentOrder);
                }
                else
                {
                    orderProcessed = TradeOrder(currentOrder, oppositeOrders);
                }
            }
            return orderProcessed;
        }

        private bool TradeOrder(Order currentOrder, List<Order> oppositeOrders)
        {
            var orderProcessed = false;
            // Existing orders with opposite action, try a trade
            if (currentOrder.Action == OrderType.Sell)
            {
                // Find all possible matches
                oppositeOrders = oppositeOrders
                    .Where(order => order.Price >= currentOrder.Price)
                    .OrderByDescending(order => order.Price)
                    .ThenBy(order => order.TimeRank)
                    .ToList();

                if (!oppositeOrders.Any())
                {
                    ExistingOrders.Add(currentOrder);
                }
                else
                {
                    orderProcessed = SellTrade(currentOrder, oppositeOrders);
                }
            }
            else if (currentOrder.Action == OrderType.Buy) // Action == Buy
            {
                // Find all possible matches
                oppositeOrders = oppositeOrders
                    .Where(order => order.Price <= currentOrder.Price)
                    .OrderBy(order => order.Price)
                    .ThenBy(order => order.TimeRank)
                    .ToList();

                if (!oppositeOrders.Any())
                {
                    ExistingOrders.Add(currentOrder);
                }
                else
                {
                    orderProcessed = BuyTrade(currentOrder, oppositeOrders);
                }
            }
            return orderProcessed;
        }

        public bool SellTrade(Order currentOrder, List<Order> oppositeOrders)
        {
            var orderProcessed = false;
            // 1. Sell entire quantity in one transaction
            // 2. Sell entire quantity in more than one transaction
            // 3. Order has quantity unsold

            // Create a Trade
            CurrentTrade = new Trade(currentOrder.AccountNumber, 0, currentOrder.Price, currentOrder.Action);
            for (var i = 0; i < oppositeOrders.Count; i++)
            {
                orderProcessed = true;
                if (currentOrder.Quantity <= oppositeOrders[i].Quantity)
                {
                    if (currentOrder.Quantity != oppositeOrders[i].Quantity)
                    {
                        // Update the other order
                        var index = ExistingOrders.IndexOf(oppositeOrders.Single(order => order.TimeRank == oppositeOrders[i].TimeRank));
                        ExistingOrders[index].Quantity -= currentOrder.Quantity;
                    }
                    else
                    {
                        // Remove the other order
                        ExistingOrders = ExistingOrders.Where(order => order.TimeRank != oppositeOrders[i].TimeRank).ToList();
                    }
                    CurrentTrade.Quantity += currentOrder.Quantity;
                    currentOrder.Quantity = 0;
                }
                // We sell more than the current match
                else
                {
                    // partial match - current order sells more than the demand
                    // Sell the amount required, delete that order, update current order
                    var tradeQuantity = oppositeOrders[i].Quantity;
                    currentOrder.Quantity -= tradeQuantity;
                    // Remove the other order
                    ExistingOrders = ExistingOrders.Where(order => order.TimeRank != oppositeOrders[i].TimeRank).ToList();
                    CurrentTrade.Quantity += tradeQuantity;
                }
            }
            // Current order was not fully processed 
            if (currentOrder.Quantity > 0)
            {
                ExistingOrders.Add(currentOrder);
            }
            return orderProcessed;
        }

        public bool BuyTrade(Order currentOrder, List<Order> oppositeOrders)
        {
            var orderProcessed = false;

            // 1. Buy the entire quantity in one transaction
            // 2. Buy the entire quantity in more than one transaction
            // 3. Order still has quantity to buy

            // Create a Trade
            CurrentTrade = new Trade(currentOrder.AccountNumber, 0, currentOrder.Price, currentOrder.Action);
            for (var i = 0; i < oppositeOrders.Count && currentOrder.Quantity > 0; i++)
            {
                orderProcessed = true;
                // We buy equal or more than the current match
                if (currentOrder.Quantity >= oppositeOrders[i].Quantity)
                {
                    // Remove order from list of existing orders
                    ExistingOrders = ExistingOrders.Where(order => order.TimeRank != oppositeOrders[i].TimeRank).ToList();
                    currentOrder.Quantity -= oppositeOrders[i].Quantity;
                    CurrentTrade.Quantity += oppositeOrders[i].Quantity;

                }
                else
                {
                    // Existing order need to be updated
                    var index = ExistingOrders.IndexOf(oppositeOrders.Single(order => order.TimeRank == oppositeOrders[i].TimeRank));
                    ExistingOrders[index].Quantity -= currentOrder.Quantity;

                    // Update the trade
                    CurrentTrade.Quantity += currentOrder.Quantity;
                    // Update current Order
                    currentOrder.Quantity = 0;
                }
                // Current order was not fully processed 
                if (currentOrder.Quantity > 0)
                {
                    ExistingOrders.Add(currentOrder);
                }
            }
            return orderProcessed;
        }
    }
}
