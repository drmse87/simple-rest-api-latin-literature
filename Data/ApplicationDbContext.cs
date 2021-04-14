using Microsoft.EntityFrameworkCore;

namespace web_api_assignment.Data
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
