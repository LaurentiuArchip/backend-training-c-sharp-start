using System;

namespace ScottLogic.Internal.Training.Matcher
{
    public class Trade : IEquatable<Trade>
    {
        private int AccountNumber;
        public int Quantity { get; set; }
        private int Price;
        private string Action;


        public Trade(int accountNumber, int quantity, int price, string action)
        {
            AccountNumber = accountNumber;
            Quantity = quantity;
            Price = price;
            Action = action;
        }

        public bool Equals(Trade other)
        {
            return other != null &&
                   AccountNumber == other.AccountNumber &&
                   Quantity == other.Quantity &&
                   Price == other.Price &&
                   Action == other.Action;
        }
    }
}
