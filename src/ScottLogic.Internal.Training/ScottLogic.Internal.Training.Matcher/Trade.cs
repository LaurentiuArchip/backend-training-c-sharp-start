using System;

namespace ScottLogic.Internal.Training.Matcher
{
    public class Trade : IEquatable<Trade>
    {
        private readonly int _accountNumber;
        private readonly int _price;
        private readonly string _action;
        public int Quantity { get; set; }


        public Trade(int accountNumber, int quantity, int price, string action)
        {
            _accountNumber = accountNumber;
            Quantity = quantity;
            _price = price;
            _action = action;
        }

        public bool Equals(Trade other)
        {
            return other != null &&
                   _accountNumber == other._accountNumber &&
                   Quantity == other.Quantity &&
                   _price == other._price &&
                   _action == other._action;
        }
    }
}
