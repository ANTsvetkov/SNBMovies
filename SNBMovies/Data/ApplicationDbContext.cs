using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SNBMovies.Models;

namespace SNBMovies.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<Actor_Movie> Actor_Movies { get; set; }
        public DbSet<Movie> Movies { get; set; }

        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {// Започва презаписване на метода OnModelCreating на базовия клас
            modelBuilder.Entity<Actor_Movie>().HasKey(am => new
            // Конфигуриране на таблицата Actor_Movie и задаване на ключ
            {
                am.ActorId,
                am.MovieId
            });
            modelBuilder.Entity<Actor_Movie>().HasOne(m => m.Movie).WithMany(am => am.Actor_Movie).HasForeignKey(m => m.MovieId);
            // Задаване на връзка между таблиците Actor_Movie и Movie, като се използва MovieId като външен ключ
            modelBuilder.Entity<Actor_Movie>().HasOne(m => m.Actor).WithMany(am => am.Actor_Movie).HasForeignKey(m => m.ActorId);
            // Задаване на връзка между таблиците Actor_Movie и Actor, като се използва ActorId като външен ключ

            base.OnModelCreating(modelBuilder);
            // Извикване на базовия метод на OnModelCreating, за да може базовият клас да конфигурира модела на базата данни
        }
    }
}
