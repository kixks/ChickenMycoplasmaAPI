using ManokDetectAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManokDetectAPI.Database
{
    public class manokDetectDBContext(DbContextOptions<manokDetectDBContext>options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Message> Messages => Set<Message>();
    }
}
