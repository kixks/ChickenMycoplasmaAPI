using ManokDetectAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManokDetectAPI.Model
{
    public class manokDetectDBContext(DbContextOptions<manokDetectDBContext>options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
    }
}
