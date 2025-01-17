using Microsoft.EntityFrameworkCore;

namespace WebApiPoc.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Models.Producto> Productos { get; set; }
        public DbSet<Models.Cliente> Clientes { get; set; }
        public DbSet<Models.Factura> Facturas { get; set; }

    }
}
