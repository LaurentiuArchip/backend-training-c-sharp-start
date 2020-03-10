using System;
using System.Collections.Generic;
using System.Text;

namespace ScottLogic.Internal.Training.Matcher
{
    public class Trade
    {
        private int accountNumber;
        public int quantity { get; set; }
        private int price;
        private string action;


        public Trade(int accountNumber, int quantity, int price, string action)
        {
            this.accountNumber = accountNumber;
            this.quantity = quantity;
            this.price = price;
            this.action = action;
        }
    }
}
