
namespace Movies.Repository.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Genera> Generas { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
    }
}
