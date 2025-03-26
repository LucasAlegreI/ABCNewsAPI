using Microsoft.EntityFrameworkCore;
using ABCNewsAPI.Models;

namespace ABCNewsAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)  // Llama al constructor base de DbContext
        {
        }

        public DbSet<ApiKey> ApiKeys { get; set; }
    }
}
