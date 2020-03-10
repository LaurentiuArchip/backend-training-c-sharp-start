using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ScottLogic.Internal.Training.Matcher
{
    public class Order : IEquatable<Order>
    {
        public int accountNumber { get; }
        public int quantity { get; set; }
        public int price { get; }
        public string action { get; }
        public int timeRank {get;}

        public Order(int accountNumber, int quantity, int price, string action, int timeRank)
        {
            this.accountNumber = accountNumber;
            this.quantity = quantity;
            this.price = price;
            this.action = action;
            this.timeRank = timeRank;
        }

        public bool Equals(Order other)
        {
            if (other == null)
            {
                return false;
            }

            return (
                this.accountNumber == other.accountNumber &&
                this.quantity == other.quantity &&
                this.price == other.price &&
                this.action == other.action);
        }
    }
}
