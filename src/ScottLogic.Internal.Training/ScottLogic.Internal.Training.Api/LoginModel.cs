using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ScottLogic.Internal.Training.Api
{
    public class User
    {
        [Key]
        public int id { get; set; }
        public string Username { get; set; }
    }
}
