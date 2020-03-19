using System;
using System.ComponentModel.DataAnnotations;

namespace ScottLogic.Internal.Training.Matcher
{
    public class Trade : IEquatable<Trade>
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please enter positive integer Number")]
        [Required]
        private int AccountNumber { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter positive integer Number")]
        [Required]
        private int Price { get; set; }
        [Required]
        private OrderType Action { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter positive integer Number")]
        [Required]
        public int Quantity { get; set; }

        public Trade(int accountNumber, int quantity, int price, OrderType action)
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
