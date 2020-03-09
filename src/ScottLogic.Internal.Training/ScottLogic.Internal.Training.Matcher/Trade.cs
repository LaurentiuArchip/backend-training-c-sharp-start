using System;
using System.Collections.Generic;
using System.Text;

namespace ScottLogic.Internal.Training.Matcher
{
    public class Trade
    {
        // something
        private Order currentOrder;


        public Trade()
        {
            currentOrder = new Order(1001, 45, 55, "buy", 12);
        }
            
    }
}
