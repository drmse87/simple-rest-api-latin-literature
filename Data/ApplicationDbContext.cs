using Microsoft.EntityFrameworkCore;

namespace latin_web_api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            //EMPTY
        }
        public DbSet<Models.Work> Works { get; set; }
        public DbSet<Models.Author> Authors { get; set; }
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Webhook> Webhooks { get; set; }
    }
}
