using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing;

namespace ScottLogic.Internal.Training.Api.Controllers
{
    public class OrderController
    {
        public ActionResult Get()
        {
            var currentMatcher = new OrderMatcher();
            return currentMatcher.ExistingOrders;
        }
    }
}
