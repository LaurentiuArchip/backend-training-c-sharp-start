using System;
using System.Collections.Generic;
using System.Text;

namespace ScottLogic.Internal.Training.Matcher
{
    public class Order
    {
        public int accountNumber { get; }
        private int quantity;
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

    }
}
