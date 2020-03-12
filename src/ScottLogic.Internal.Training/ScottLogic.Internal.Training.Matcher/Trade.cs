using System;
using System.ComponentModel.DataAnnotations;

namespace ScottLogic.Internal.Training.Matcher
{
    public class Trade : IEquatable<Trade>
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please enter positive integer Number")]
        private readonly int _accountNumber;
        [Range(1, int.MaxValue, ErrorMessage = "Please enter positive integer Number")]
        private readonly int _price;
        [Required]
        private readonly OrderType _action;
        [Range(1, int.MaxValue, ErrorMessage = "Please enter positive integer Number")]
        public int Quantity { get; set; }

        public Trade(int accountNumber, int quantity, int price, OrderType action)
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
