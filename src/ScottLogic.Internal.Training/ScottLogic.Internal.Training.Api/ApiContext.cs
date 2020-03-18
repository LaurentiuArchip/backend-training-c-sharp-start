using Microsoft.EntityFrameworkCore;

namespace ScottLogic.Internal.Training.Api
{
    public class ApiContext: DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
