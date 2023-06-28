using Microsoft.EntityFrameworkCore;

namespace StreamingWithBackpressure
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
         : base(options)
        {
            
        }
        
    }
}
