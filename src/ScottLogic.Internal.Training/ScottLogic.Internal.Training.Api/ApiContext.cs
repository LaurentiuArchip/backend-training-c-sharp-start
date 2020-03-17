using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScottLogic.Internal.Training.Api
{
    public class ApiContext: DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
            LoadUsers();
        }

        public DbSet<User> Users { get; set; }

        public void LoadUsers()
        {
            var user = new User() {Username = "Paul"};
            Users.Add(user);
            SaveChanges();
        }
    }
}
