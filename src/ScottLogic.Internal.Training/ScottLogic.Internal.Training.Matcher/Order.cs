using System;
using System.Collections.Generic;
using System.Text;

namespace ScottLogic.Internal.Training.Matcher
{
    class Order
    {
        private int accountNumber;
        private int quantity;
        private int price;

        public Order(int accountNumber, int quantity, int price)
        {
            this.accountNumber = accountNumber;
            this.quantity = quantity;
            this.price = price;
        }

    }
}
