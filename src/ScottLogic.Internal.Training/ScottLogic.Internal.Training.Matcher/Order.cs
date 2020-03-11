using System;

namespace ScottLogic.Internal.Training.Matcher
{
    public enum OrderType
    {
        None,
        Buy,
        Sell
    }
    public class Order : IEquatable<Order>
    {
        public int AccountNumber { get; }
        public int Quantity { get; set; }
        public int Price { get; }
        public OrderType Action { get; }
        public int TimeRank {get; }

        public Order() { }
        public Order(int accountNumber, int quantity, int price, OrderType action, int timeRank)
        {
            AccountNumber = accountNumber;
            Quantity = quantity;
            Price = price;
            Action = action;
            TimeRank = timeRank;
        }

        public bool Equals(Order other)
        {
            return other != null && 
                AccountNumber == other.AccountNumber &&
                Quantity == other.Quantity &&
                Price == other.Price &&
                Action == other.Action &&
                TimeRank == other.TimeRank;
        }
    }
}
